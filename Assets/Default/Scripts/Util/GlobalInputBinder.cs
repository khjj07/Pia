using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;

namespace Default.Scripts.Util
{
    public class GlobalInputBinder : Singleton<GlobalInputBinder>
    {
        public static IObservable<Vector3> CreateGetMousePositionStream()
        {
            return Instance.UpdateAsObservable().Select(_ => Input.mousePosition);
        }

        public static IObservable<float> CreateGetAxisStream(string axis)
        {
            return Instance.FixedUpdateAsObservable().Select(_ => Input.GetAxis(axis));
        }

        public static IObservable<float> CreateGetAxisStreamOptimize(string axis)
        {
            return Instance.FixedUpdateAsObservable().Select(_ => Input.GetAxis(axis)).Where(x => x != 0);
        }

        public static IObservable<bool> CreateGetKeyStream(KeyCode key)
        {
            return Instance.UpdateAsObservable().Where(_ => Input.GetKey(key)).Select(_ => Input.GetKey(key));
        }

        public static IObservable<Unit> CreateGetKeyDownStream(KeyCode key)
        {
            return Instance.UpdateAsObservable().Where(_ => Input.GetKeyDown(key));
        }

        public static IObservable<Unit> CreateGetKeyUpStream(KeyCode key)
        {
            return Instance.UpdateAsObservable().Where(_ => Input.GetKeyUp(key));
        }

        public static IObservable<bool> CreateGetMouseButtonStream(int btn)
        {
            return Instance.UpdateAsObservable().Where(_ => Input.GetMouseButton(btn))
                .Select(_ => Input.GetMouseButton(btn));
        }

        public static IObservable<bool> CreateGetMouseButtonDownStream(int btn)
        {
            return Instance.UpdateAsObservable().Where(_ => Input.GetMouseButtonDown(btn))
                .Select(_ => Input.GetMouseButtonDown(btn));
        }

        public static IObservable<bool> CreateGetMouseButtonUpStream(int btn)
        {
            return Instance.UpdateAsObservable().Where(_ => Input.GetMouseButtonUp(btn))
                .Select(_ => Input.GetMouseButtonUp(btn));
        }

        public static void CreateGetEventAxisStream(string axis, UnityEvent e)
        {
            Instance.UpdateAsObservable().Select(_ => Input.GetAxis(axis)).Subscribe(_ => e.Invoke());
        }

        public static void CreateEventKeyStream(KeyCode key, UnityEvent e)
        {
            Instance.UpdateAsObservable().Where(_ => Input.GetKey(key)).Subscribe(_ => e.Invoke());
        }

        public static void CreateEventKeyDownStream(KeyCode key, UnityEvent e)
        {
            Instance.UpdateAsObservable().Where(_ => Input.GetKeyDown(key)).Subscribe(_ => e.Invoke());
        }

        public static void CreateEventKeyUpStream(KeyCode key, UnityEvent e)
        {
            Instance.UpdateAsObservable().Where(_ => Input.GetKeyUp(key)).Subscribe(_ => e.Invoke());
        }

        public static void CreateGetMouseButtonStream(int btn, UnityEvent e)
        {
            Instance.UpdateAsObservable().Where(_ => Input.GetMouseButton(btn)).Subscribe(_ => e.Invoke());
        }

        public static void CreateGetMouseButtonDownStream(int btn, UnityEvent e)
        {
            Instance.UpdateAsObservable().Where(_ => Input.GetMouseButtonDown(btn)).Subscribe(_ => e.Invoke());
        }

        public static void CreateGetMouseButtonUpStream(int btn, UnityEvent e)
        {
            Instance.UpdateAsObservable().Where(_ => Input.GetMouseButtonUp(btn)).Subscribe(_ => e.Invoke());
        }
    }
}