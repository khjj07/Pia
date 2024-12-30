using System;
using UnityEditor;
using UnityEngine;

namespace Default.Scripts.Util
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    try
                    {
                        _instance = FindAnyObjectByType<T>(FindObjectsInactive.Include);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError(e.StackTrace);
                        return null;
                    }
                }

                return _instance;
            }
        }
    }
    public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance;

        public static T instance
        {
            get
            {
                if (_instance == null)
                    try
                    {
                        var type = typeof(T).Name;
#if UNITY_EDITOR

                        var guid = AssetDatabase.FindAssets("t:" + type);
                        var path = AssetDatabase.GUIDToAssetPath(guid[0]);
                        _instance = (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
#else
                        _instance = (T)Resources.Load<T>(type);
#endif

                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError(e.StackTrace);
                        return null;
                    }

                return _instance;
            }
        }
    }
}