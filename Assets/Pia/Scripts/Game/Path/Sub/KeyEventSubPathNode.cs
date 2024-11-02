using System.Threading;
using System.Threading.Tasks;
using Default.Scripts.Util;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Pia.Scripts.Path.Sub
{
    public class KeyEventSubPathNode : SubPathNode
    {
        public KeyCode nextKey = KeyCode.Tab;
        public UnityEvent keyPressedEvent;
        public override async Task Appear(CancellationTokenSource cancellationTokenSource)
        {
            gameObject.SetActive(true);
            await WaitForKeyPress(nextKey);
            keyPressedEvent.Invoke();
            gameObject.SetActive(false);
        }

        public override Task Disappear(CancellationTokenSource cancellationTokenSource)
        {
            return Task.CompletedTask;
        }

        private Task WaitForKeyPress(KeyCode key)
        {
            var tcs = new TaskCompletionSource<bool>();

            void CheckKeyInput()
            {
                if (Input.GetKeyDown(key))
                {
                    tcs.SetResult(true);
                }
            }

            GlobalInputBinder.CreateGetKeyDownStream(key)
                .Take(1)
                .Subscribe(_ => CheckKeyInput())
                .AddTo(gameObject);

            return tcs.Task;
        }
    }
}