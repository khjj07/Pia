﻿using System;
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

        public Image optionPanel;
        public Button optionConfirmButton;
        public Button optionResetButton;

        public Slider exposureSlider;
        public Slider mouseSensitiveSlider;
        public Slider volumeSlider;
        public Toggle motionBlurToggle;
        public Toggle headBobToggle;
        public Toggle pedalToggle;


        public TMP_Dropdown languageDropdown;

        private void Start()
        {
            startButton.onClick.AddListener(OnStartButtonClick);
            optionButton.onClick.AddListener(OnOptionButtonClick);
            quitButton.onClick.AddListener(OnQuitButtonClick);
            optionConfirmButton.onClick.AddListener(OnOptionConfirmButtonClick);
            optionResetButton.onClick.AddListener(OnOptionResetButtonClick);
            languageDropdown.onValueChanged.AddListener(OnLanguageDropdownChanged);
          

            GlobalConfiguration.Instance.LoadAllProperty();
            languageDropdown.value = GlobalConfiguration.Instance.GetLanguage();
            InitializeOption();
            PlayerPrefs.DeleteKey("Save");
            SoundManager.Play("BGM_Title");
            GlobalConfiguration.Instance.SetFog(false);
            Cursor.lockState= CursorLockMode.None;
        }

        private void InitializeOption()
        {
            exposureSlider.value = GlobalConfiguration.Instance.GetExposure();
            motionBlurToggle.isOn = GlobalConfiguration.Instance.GetMotionBlur();
            headBobToggle.isOn = GlobalConfiguration.Instance.GetHeadBob();
            pedalToggle.isOn = GlobalConfiguration.Instance.GetPedalUse();
            mouseSensitiveSlider.value = GlobalConfiguration.Instance.GetMouseSensitive();
            volumeSlider.value = GlobalConfiguration.Instance.GetVolume();

            motionBlurToggle.onValueChanged.AddListener(GlobalConfiguration.Instance.SetMotionBlur);
            headBobToggle.onValueChanged.AddListener(GlobalConfiguration.Instance.SetHeadBob);
            pedalToggle.onValueChanged.AddListener(GlobalConfiguration.Instance.SetPedalUse);
            mouseSensitiveSlider.onValueChanged.AddListener(GlobalConfiguration.Instance.SetMouseSensitive);
            volumeSlider.onValueChanged.AddListener(GlobalConfiguration.Instance.SetVolume);
            exposureSlider.onValueChanged.AddListener(GlobalConfiguration.Instance.SetExposure);
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

        private void OnLanguageDropdownChanged(int value)
        {
            GlobalConfiguration.Instance.SetLanguage(value);
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