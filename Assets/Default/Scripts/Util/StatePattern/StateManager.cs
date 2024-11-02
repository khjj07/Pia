using System;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace Default.Scripts.Util.StatePattern
{
#if UNITY_EDITOR
    [CustomEditor(typeof(StateManagerBase), true)]
    public class StateManagerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!Application.isPlaying)
            {
                var sm = (StateManagerBase)target;
                sm.states = sm.GetComponentsInChildren<StateBase>(true);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Previous Node"))
                {
                    sm.Previous();
                }
                if (GUILayout.Button("Next Node"))
                {
                    sm.Next();
                }
                GUILayout.EndHorizontal();
            }
        }
    }
#endif
    public abstract class StateManagerBase : MonoBehaviour
    {
        public StateBase[] states;
        public int currentIndex;
        public abstract void Next();
        public abstract void Previous();
        public abstract void Change(StateBase state);
    }

    public abstract class StateManager<T> : StateManagerBase where T : State<T>
    {
        public T currentState;
        protected CancellationTokenSource _cancellationTokenSource;
        public virtual void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            states = GetComponentsInChildren<T>(true);
            foreach (var state in states)
            {
                state.gameObject.SetActive(false);
            }
            if (states.Length > 0)
            {
                currentState = states[currentIndex] as T;
                if (currentState != null)
                {
                    currentState.gameObject.SetActive(true);
                    currentState.OnEnter(_cancellationTokenSource);
                }
            }
        }

        public override void Next()
        {
            if (currentIndex < states.Length - 1)
            {
                currentIndex++;
                Change(states[currentIndex]);
            }
#if UNITY_EDITOR
            else
            {
                UnityEngine.Debug.Log("Out Of Range");
            }
#endif
        }

        public override void Previous()
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                Change(states[currentIndex]);
            }
#if UNITY_EDITOR
            else
            {
                UnityEngine.Debug.Log("Out Of Range");
            }
#endif
        }

        public override async void Change(StateBase state)
        {
            await currentState.OnExit(_cancellationTokenSource);
            currentState = state as T;
            await currentState.OnEnter(_cancellationTokenSource);
        }
    }
}