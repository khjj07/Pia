using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Default.Scripts.Util.StatePattern
{
    public abstract class StateBase : MonoBehaviour
    {
        public abstract Task OnEnter(CancellationTokenSource tokenSource);
        public abstract Task OnExit(CancellationTokenSource tokenSource);
    }

    public abstract class State<T> : StateBase
    {
        public override Task OnEnter(CancellationTokenSource tokenSource)
        {
           gameObject.SetActive(true);
           return Task.CompletedTask;
        }

        public override Task OnExit(CancellationTokenSource tokenSource)
        {
             gameObject.SetActive(false);
             return Task.CompletedTask;
        }
    }
}