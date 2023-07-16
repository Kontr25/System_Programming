using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Lesson_2.homework_1
{
    public class FirstTask : MonoBehaviour
    {
        [SerializeField] private int[] _numberArray;
        private NativeArray<int> _numbers;

        private void Start()
        {
            _numbers = new NativeArray<int>(_numberArray.Length, Allocator.TempJob);
            for (int i = 0; i < _numberArray.Length; i++)
            {
                _numbers[i] = _numberArray[i];
            }
            
            MyJob1 myJob1 = new MyJob1()
            {
                numbers = _numbers
            };

            JobHandle jobHandle = myJob1.Schedule();
            jobHandle.Complete();
            _numbers.Dispose();
        }
    }
    
    public struct MyJob1: IJob
    {
        public NativeArray<int> numbers;
        public void Execute()
        {
            int result = 0;
            for (int i = 0; i < numbers.Length; i++)
            {
                result = numbers[i];
                if (result > 10)
                {
                    result = 0;
                }
                Debug.Log($"{i+1}. {result}");
            }
        }
    }
}