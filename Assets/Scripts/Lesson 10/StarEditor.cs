using UnityEditor;
using UnityEngine;

namespace Lesson_10
{
    [CustomEditor(typeof(Star)), CanEditMultipleObjects]
    public class StarEditor : Editor
    {
        private SerializedProperty _center;
        private SerializedProperty _points;
        private SerializedProperty _frequency;

        private Vector3 _pointSnap = new Vector3();
        private void OnEnable()
        {
            _center = serializedObject.FindProperty("_center");
            _points = serializedObject.FindProperty("_points");
            _frequency = serializedObject.FindProperty("_frequency");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_center);
            EditorGUILayout.PropertyField(_points);
            EditorGUILayout.IntSlider(_frequency, 1, 20);
            
            var totalPoints = _frequency.intValue * _points.arraySize;
            
            if (totalPoints < 3)
            {
                EditorGUILayout.HelpBox("At least three points are needed.", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox(totalPoints + " points in total.", MessageType.Info);
            }
            serializedObject.ApplyModifiedProperties();
        }
        
        private void OnSceneGUI()
        {
            if (!(target is Star star))
            {
                return;
            }
            var starTransform = star.transform;
            var angle = -360f / (star._frequency * star._points.Length);
            for (var i = 0; i < star._points.Length; i++)
            {
                var rotation = Quaternion.Euler(0f, 0f, angle * i);
                var oldPoint = starTransform.TransformPoint(rotation * star._points[i].Position);
                var newPoint = Handles.FreeMoveHandle(oldPoint, Quaternion.identity,
                    0.02f, _pointSnap, Handles.DotHandleCap);
                if (oldPoint == newPoint)
                {
                    continue;
                }
                star._points[i].Position = Quaternion.Inverse(rotation) * starTransform.InverseTransformPoint(newPoint);
                star.UpdateMesh();
            }
        }
    }
}