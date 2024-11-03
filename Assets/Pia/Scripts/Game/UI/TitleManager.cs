using System;
using Default.Scripts.Sound;
using Default.Scripts.Util;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Pia.Scripts.Manager
{
    public class TitleManager : MonoBehaviour
    {
        public Button startButton;
        public Button quitButton;

        private void Start()
        {
            startButton.onClick.AddListener(OnStartButtonClick);
            quitButton.onClick.AddListener(OnQuitButtonClick);
            PlayerPrefs.DeleteKey("Save");
            SoundManager.Play("BGM_Title");
        }

        private void OnStartButtonClick()
        {
            SoundManager.PlayOneShot("ui_button",1);
            StartCoroutine(StoryModeLoadingManager.Instance.Load("StoryModeSynopsis", 1.0f));
        }

        private void OnQuitButtonClick()
        {
            SoundManager.PlayOneShot("ui_button", 1);
            Observable.Timer(TimeSpan.FromSeconds(0.1f)).Subscribe(_ =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
            }).AddTo(gameObject);
        }
    }
}