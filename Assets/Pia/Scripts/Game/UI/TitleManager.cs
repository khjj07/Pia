using System;
using Assets.Pia.Scripts.Game.UI;
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
        public Button optionButton;
        public Button quitButton;

        public Image optionPanel;
        public Button optionConfirmButton;
        public Button optionResetButton;

        public Slider exposureSlider;
        public Slider mouseSensitiveSlider;
        public Slider volumeSlider;
        public Toggle motionBlurToggle;
        public Toggle headBobToggle;

        private void Start()
        {
            startButton.onClick.AddListener(OnStartButtonClick);
            optionButton.onClick.AddListener(OnOptionButtonClick);
            quitButton.onClick.AddListener(OnQuitButtonClick);
            optionConfirmButton.onClick.AddListener(OnOptionConfirmButtonClick);
            optionResetButton.onClick.AddListener(OnOptionResetButtonClick);
            InitializeOption();
            PlayerPrefs.DeleteKey("Save");
            SoundManager.Play("BGM_Title");
            GlobalConfiguration.Instance.SetFog(false);
        }

        private void InitializeOption()
        {
            exposureSlider.value = GlobalConfiguration.Instance.GetExposure();
            motionBlurToggle.isOn = GlobalConfiguration.Instance.GetMotionBlur();
            headBobToggle.isOn = GlobalConfiguration.Instance.GetHeadBob();
            mouseSensitiveSlider.value = GlobalConfiguration.Instance.GetMouseSensitive();
            volumeSlider.value = GlobalConfiguration.Instance.GetVolume();
        }

        private void OnOptionResetButtonClick()
        {
            SoundManager.PlayOneShot("ui_button", 1);
            PlayerPrefs.DeleteAll();
            GlobalConfiguration.Instance.LoadAllProperty();
            InitializeOption();
        }

        private void OnOptionConfirmButtonClick()
        {
            SoundManager.PlayOneShot("ui_button", 1);
            optionPanel.gameObject.SetActive(false);
            PlayerPrefs.Save();
        }

        private void OnOptionButtonClick()
        {
            SoundManager.PlayOneShot("ui_button", 1);
            optionPanel.gameObject.SetActive(true);
        }

        private void OnStartButtonClick()
        {
            SoundManager.PlayOneShot("ui_button",1);
            StartCoroutine(StoryModeLoadingManager.Instance.Load("StoryModeSynopsis"));
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