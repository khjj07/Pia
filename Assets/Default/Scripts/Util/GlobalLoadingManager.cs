using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Default.Scripts.Util
{
    public class GlobalLoadingManager : Singleton<GlobalLoadingManager>
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void OnSceneLoaded()
        {

        }
        public static IEnumerator Load(string scene, float defaultDelay = 0.0f)
        {
            UnityEngine.Debug.Log("Load " + scene);
            //var previousScene = SceneManager.GetActiveScene();
            var loadOperation = SceneManager.LoadSceneAsync(scene);
            loadOperation.allowSceneActivation = false;
            Instance.OnLoadBegin();
            yield return new WaitForSeconds(defaultDelay);
            yield return new WaitUntil(() => loadOperation.progress >= 0.9f);
            loadOperation.allowSceneActivation = true;
            Instance.OnLoadEnd();
        }

        protected virtual void OnLoadEnd()
        {
            
        }

        protected virtual void OnLoadBegin()
        {
           
        }
    }
}