using System;
using System.Collections;
using System.Threading.Tasks;
using Default.Scripts.Sound;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Default.Scripts.Util
{
    public class GlobalLoadingManager : Singleton<GlobalLoadingManager>
    {
        public Subject<string> sceneSubject;
        public enum Mode
        {
            Fade,
            None
        }

        public void Start()
        {
            sceneSubject = new Subject<string>();
            sceneSubject.Subscribe(x => LoadScene(x)).AddTo(gameObject);
        }
        public void LoadScene(string scene, float defaultDelay = 1.0f, Mode mode = Mode.Fade, bool stopSound = false)
        {
            StartCoroutine(Load(scene));
        }

        public IEnumerator Load(string scene,float defaultDelay = 1.0f, Mode mode = Mode.Fade,bool stopSound = false)
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
            if (stopSound)
            {
                SoundManager.StopAll();
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