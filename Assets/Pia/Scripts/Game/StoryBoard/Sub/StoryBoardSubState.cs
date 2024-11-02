using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Pia.Scripts.StoryMode.Synopsis.Sub
{
    public class StoryBoardSubState : MonoBehaviour
    {
        public float appearDelay = 0.0f;
        public virtual async Task Appear(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                await Task.Delay((int)(appearDelay * 1000), cancellationTokenSource.Token);
                gameObject.SetActive(true);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Async task was canceled.");
            }
           
        }
    }
}
