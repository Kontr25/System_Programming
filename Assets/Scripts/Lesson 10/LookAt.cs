using UnityEngine;
using UnityEngine.Serialization;

namespace Lesson_10
{
    [ExecuteInEditMode]
    public class LookAt : MonoBehaviour
    {
        public Vector3 lookAtPoint = Vector3.zero;
        public void Update()
        {
            transform.LookAt(lookAtPoint);
        }
    }

}