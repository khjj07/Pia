using System;
using System.Collections;
using System.Threading.Tasks;
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
            
        }

        public IEnumerator Load(string scene, float defaultDelay = 0.0f)
        {
            var loadOperation = SceneManager.LoadSceneAsync(scene);
            loadOperation.allowSceneActivation = false;
            yield return OnLoadBegin();
            yield return new WaitForSeconds(defaultDelay);
            yield return new WaitUntil(() => loadOperation.progress >= 0.9f);
            loadOperation.allowSceneActivation = true;
            yield return OnLoadEnd();
        }

        protected virtual IEnumerator OnLoadEnd()
        {
            yield return null;
        }

        protected virtual IEnumerator OnLoadBegin()
        {
            yield return null;
        }
    }
}