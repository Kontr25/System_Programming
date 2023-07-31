using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;

namespace Lesson_9
{
    public class Fractal2 : MonoBehaviour
    {
        struct FractalPart
        {
            public Vector3 Direction;
            public Quaternion Rotation;
            public Vector3 WorldPosition;
            public Quaternion WorldRotation;
            public float SpinAngle;
        }
        
        private struct UpdateFractalLevelJob: IJobParallelFor
        {
            public float SpinAngleDelta;
            public float Scale;
            [ReadOnly]
            public NativeArray<FractalPart> Parents;
            public NativeArray<FractalPart> Parts;
            [WriteOnly]
            public NativeArray<Matrix4x4> Matrices;
            
            [BurstCompile]
            public void Execute(int index)
            {
                var parent = Parents[index / _childCount];
                var part = Parts[index];
                part.SpinAngle += SpinAngleDelta;
                part.WorldRotation =
                    parent.WorldRotation *
                    (part.Rotation * Quaternion.Euler(0f, part.SpinAngle, 0f));
                part.WorldPosition =
                    parent.WorldPosition +
                    parent.WorldRotation * (_positionOffset * Scale *
                                            part.Direction);
                Parts[index] = part;
                Matrices[index] = Matrix4x4.TRS(
                    part.WorldPosition, part.WorldRotation, Scale * Vector3.one
                );
            }
        }

        [SerializeField] private Mesh _mesh;
        [SerializeField] private Material _material;
        [SerializeField, Range(1, 8)] private int _depth = 4;
        [SerializeField, Range(0, 360)] private int _speedRotation = 80;
        private const float _positionOffset = 1.5f;
        private const float _scaleBias = .5f;
        private const int _childCount = 5;
        private NativeArray<FractalPart>[] _parts;
        private NativeArray<Matrix4x4>[] _matrices;
        private NativeArray<JobHandle> _handles;
        private ComputeBuffer[] _matricesBuffers;
        private static readonly int _matricesId = Shader.PropertyToID("_Matrices");
        private static MaterialPropertyBlock _propertyBlock;

        private static readonly Vector3[] _directions =
        {
            Vector3.up,
            Vector3.left,
            Vector3.right,
            Vector3.forward,
            Vector3.back
        };

        private static readonly Quaternion[] _rotations =
        {
            Quaternion.identity,
            Quaternion.Euler(.0f, .0f, 90.0f),
            Quaternion.Euler(.0f, .0f, -90.0f),
            Quaternion.Euler(90.0f, .0f, .0f),
            Quaternion.Euler(-90.0f, .0f, .0f)
        };

        private void OnEnable()
        {
            if (_parts == null || _matrices == null || _matricesBuffers == null)
            {
                _parts = new NativeArray<FractalPart>[_depth];
                _matrices = new NativeArray<Matrix4x4>[_depth];
                _matricesBuffers = new ComputeBuffer[_depth];
                var stride = 16 * 4;
                for (int i = 0, length = 1;
                     i < _parts.Length;
                     i++, length *=
                         _childCount)
                {
                    _parts[i] = new NativeArray<FractalPart>(length, Allocator.Persistent);
                    _matrices[i] = new NativeArray<Matrix4x4>(length, Allocator.Persistent);
                    _matricesBuffers[i] = new ComputeBuffer(length, stride);
                }

                _parts[0][0] = CreatePart(0);
                for (var li = 1; li < _parts.Length; li++)
                {
                    var levelParts = _parts[li];
                    for (var fpi = 0; fpi < levelParts.Length; fpi += _childCount)
                    {
                        for (var ci = 0; ci < _childCount; ci++)
                        {
                            levelParts[fpi + ci] = CreatePart(ci);
                        }
                    }
                }
            }

            _handles = new NativeArray<JobHandle>(_parts.Length - 1, Allocator.Persistent);
            _propertyBlock ??= new MaterialPropertyBlock();
        }

        private void OnDisable()
        {
            for (var i = 0; i < _matricesBuffers.Length; i++)
            {
                _matricesBuffers[i].Release();
                _parts[i].Dispose();
                _matrices[i].Dispose();
            }

            _handles.Dispose();
            _parts = null;
            _matrices = null;
            _matricesBuffers = null;
        }

        private void OnValidate()
        {
            if (_parts is null || !enabled)
            {
                return;
            }

            OnDisable();
            OnEnable();
        }

        private FractalPart CreatePart(int childIndex) => new FractalPart
        {
            Direction = _directions[childIndex],
            Rotation = _rotations[childIndex],
        };

        private void Update()
        {
            var spinAngelDelta = _speedRotation * Time.deltaTime;
            var rootPart = _parts[0][0];
            rootPart.SpinAngle += spinAngelDelta;
            var deltaRotation = Quaternion.Euler(.0f, rootPart.SpinAngle, .0f);
            rootPart.WorldRotation = rootPart.Rotation * deltaRotation;
            _parts[0][0] = rootPart;
            _matrices[0][0] = Matrix4x4.TRS(rootPart.WorldPosition,
                rootPart.WorldRotation, Vector3.one);
            var scale = 1.0f;

            JobHandle finalHandle = default;

            for (var li = 1; li < _parts.Length; li++)
            {
                scale *= _scaleBias;
                var job = new UpdateFractalLevelJob
                {
                    SpinAngleDelta = spinAngelDelta,
                    Scale = scale,
                    Parents = _parts[li - 1],
                    Parts = _parts[li],
                    Matrices = _matrices[li]
                };
                _handles[li - 1] = job.Schedule(_parts[li].Length, 64, finalHandle);
                finalHandle = _handles[li - 1];
            }

            JobHandle.CompleteAll(_handles);
            
            var bounds = new Bounds(rootPart.WorldPosition, 3f * Vector3.one);
            for (var i = 0; i < _matricesBuffers.Length; i++)
            {
                var buffer = _matricesBuffers[i];
                buffer.SetData(_matrices[i]);
                _propertyBlock.SetBuffer(_matricesId, buffer);
                _material.SetBuffer(_matricesId, buffer);
                Graphics.DrawMeshInstancedProcedural(_mesh, 0, _material, bounds,
                    buffer.count, _propertyBlock);
            }
        }
    }
}
