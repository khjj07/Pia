using System.Threading.Tasks;
using Default.Scripts.Util;
using UniRx;
using UnityEngine;

namespace Assets.Pia.Scripts.Path.Sub
{
 
    public class WaitKeyPathNode : SubPathNode
    {
        public KeyCode nextKey = KeyCode.Tab;

        public async override Task Appear()
        {
            await WaitForKeyPress(nextKey);
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