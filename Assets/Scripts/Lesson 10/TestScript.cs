using UnityEngine;

namespace Lesson_10
{
    public class TestScript : MonoBehaviour
    {
        [UnityEngine.Range(0, 20), SerializeField] private int _integer;
        [UnityEngine.Range(0f, 20f), SerializeField] private float _float;
        [UnityEngine.Range(0f, 20), SerializeField] private string _string;
    }
}