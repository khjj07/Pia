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
        public enum Mode
        {
            Fade,
            None
        }

        public IEnumerator Load(string scene,float defaultDelay = 0.0f, Mode mode = Mode.Fade)
        {
            if (mode == Mode.Fade)
            {
                yield return OnLoadBegin(mode);
            }
            var loadOperation = SceneManager.LoadSceneAsync(scene);
            loadOperation.allowSceneActivation = false;
            yield return new WaitForSeconds(defaultDelay);
            yield return new WaitUntil(() => loadOperation.progress >= 0.9f);
            loadOperation.allowSceneActivation = true;
            if (mode == Mode.Fade)
            {
                yield return OnLoadEnd(mode);
            }
        }

        protected virtual IEnumerator OnLoadEnd(Mode mode)
        {
            yield return null;
        }

        protected virtual IEnumerator OnLoadBegin(Mode mode)
        {
            yield return null;
        }
    }
}