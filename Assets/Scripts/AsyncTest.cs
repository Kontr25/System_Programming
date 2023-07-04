using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace DefaultNamespace
{
    public class AsyncTest : MonoBehaviour
    {
        private CancellationTokenSource _cancelTokenSource;
        private CancellationToken _cancelToken;
        private Task _task1;
        private Task _task2;
        private void Start()
        {
            _cancelTokenSource = new CancellationTokenSource();
            _cancelToken = _cancelTokenSource.Token;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ExampleAsync(_cancelToken);
            }
            if (Input.GetMouseButtonDown(1))
            {
                _cancelTokenSource.Cancel();
            }
        }

        async void ExampleAsync(CancellationToken cancellationToken)
        {
            await Task.WhenAll(Task1Async(_cancelToken), Task2Async(_cancelToken));
            print("All tasks complete");
        }

        async Task Task1Async(CancellationToken cancellationToken)
        {
            await Task.Delay(5000);
            if (CancelTask()) return;
            print("Task 1 complete");
        }
        
        async Task Task2Async(CancellationToken cancellationToken)
        {
            int frame = 0;
            while (frame < 500)
            {
                if (CancelTask()) return;
                frame++;
                await Task.Yield();
            }
            print("Task 2 complete");
        }

        private bool CancelTask()
        {
            if (_cancelToken.IsCancellationRequested)
            {
                _cancelTokenSource.Dispose();
                return true;
            }

            return false;
        }
    }
}