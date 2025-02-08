using System;
using Assets.Pia.Scripts.Game.UI;
using Default.Scripts.Sound;
using Default.Scripts.Util;
using TMPro;
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
        public Button discordButton;

        private OptionManager optionManager;

        public TMP_Dropdown languageDropdown;

        private void Start()
        {
            startButton.onClick.AddListener(OnStartButtonClick);
            optionButton.onClick.AddListener(OnOptionButtonClick);
            quitButton.onClick.AddListener(OnQuitButtonClick);
            discordButton.onClick.AddListener(OnDiscordButtonClick);

            languageDropdown.onValueChanged.AddListener(OnLanguageDropdownChanged);
            GlobalConfiguration.Instance.LoadAllProperty();
            languageDropdown.value = GlobalConfiguration.Instance.GetLanguage();
       
            PlayerPrefs.DeleteKey("Save");
            SoundManager.Play("BGM_Title");
            GlobalConfiguration.Instance.SetFog(false);
            Cursor.lockState= CursorLockMode.None;
           
        }

        private void OnDiscordButtonClick()
        {
            Application.OpenURL("https://discord.gg/TkTcEyRHbh");
        }

        private void OnOptionButtonClick()
        {
            SoundManager.PlayOneShot("ui_button", 1);
            if (!optionManager)
            {
                optionManager = GetComponentInChildren<OptionManager>();
                optionManager.Initialize();
            }
            optionManager.Open();
        }

        private void OnStartButtonClick()
        {
            SoundManager.PlayOneShot("ui_button",1);
            StoryModeLoadingManager.Instance.LoadScene("StoryModeSynopsis");
        }

        private void OnLanguageDropdownChanged(int value)
        {
            GlobalConfiguration.Instance.SetLanguage(value);
            optionManager = null;
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