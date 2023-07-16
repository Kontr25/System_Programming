using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Lesson_2.homework_1
{
    public class SecondTask : MonoBehaviour
    {
        [SerializeField] private Vector3[] _positions;
        [SerializeField] private Vector3[] _velocities;
        
        private NativeArray<Vector3> _positionsArray;
        private NativeArray<Vector3> _velocitiesArray;
        private NativeArray<Vector3> _finalPositionsArray;

        private void Start()
        {
            InitializeArrays();

            CalculateFinalPositions();
        }

        private void InitializeArrays()
        {
            _positionsArray = new NativeArray<Vector3>(_positions.Length, Allocator.TempJob);
            for (int i = 0; i < _positions.Length; i++)
            {
                _positionsArray[i] = _positions[i];
            }

            _velocitiesArray = new NativeArray<Vector3>(_velocities.Length, Allocator.TempJob);
            for (int i = 0; i < _velocities.Length; i++)
            {
                _velocitiesArray[i] = _velocities[i];
            }

            int finalPositionsLength = Int32.MaxValue;
            if (_positions.Length < finalPositionsLength) finalPositionsLength = _positions.Length;
            if (_velocities.Length < finalPositionsLength) finalPositionsLength = _velocities.Length;

            _finalPositionsArray = new NativeArray<Vector3>(finalPositionsLength, Allocator.TempJob);
        }
        
        
        private void CalculateFinalPositions()
        {
            MyJob2 myJob2 = new MyJob2()
            {
                positionsArray = _positionsArray,
                velocitiesArray = _velocitiesArray,
                finalPositionsArray = _finalPositionsArray
            };

            JobHandle jobHandle = myJob2.Schedule(_finalPositionsArray.Length, 0);
            jobHandle.Complete();

            for (int i = 0; i < _finalPositionsArray.Length; i++)
            {
                Debug.Log($"{i}. Finalposition = {_finalPositionsArray[i]}");
            }
        }

        private void OnDestroy()
        {
            _positionsArray.Dispose();
            _velocitiesArray.Dispose();
            _finalPositionsArray.Dispose();
        }
    }
    
    
    public struct MyJob2: IJobParallelFor
    {
        public NativeArray<Vector3> positionsArray;
        public NativeArray<Vector3> velocitiesArray;
        public NativeArray<Vector3> finalPositionsArray;
        public void Execute(int index)
        {
            finalPositionsArray[index] = positionsArray[index] + velocitiesArray[index];
        }
    }
}