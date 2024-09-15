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
        public async override Task Appear()
        {
            await WaitForKeyPress(nextKey);
            keyPressedEvent.Invoke();
        }

        public override Task Disappear()
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
                .First()
                .Subscribe(_ => CheckKeyInput())
                .AddTo(gameObject);

            return tcs.Task;
        }
    }
}