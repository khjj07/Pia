using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Default.Scripts.Util.StatePattern
{
    public abstract class StateBase : MonoBehaviour
    {
        public abstract Task OnEnter();
        public abstract Task OnExit();
    }

    public abstract class State<T> : StateBase
    {
        public override Task OnEnter()
        {
           gameObject.SetActive(true);
           return Task.CompletedTask;
        }

        public override Task OnExit()
        {
             gameObject.SetActive(false);
             return Task.CompletedTask;
        }
    }
}