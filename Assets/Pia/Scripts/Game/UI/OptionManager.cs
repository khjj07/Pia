using Default.Scripts.Sound;
using Pia.Scripts.Manager;
using System;
using Default.Scripts.Util;
using Pia.Scripts.StoryMode;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Pia.Scripts.Game.UI
{
    public class OptionManager : MonoBehaviour
    {

        public Image optionPanel;
        public Button optionConfirmButton;
        public Button optionResetButton;

        public Slider exposureSlider;
        public Slider mouseSensitiveSlider;
        public Slider volumeSlider;
        public Toggle motionBlurToggle;
        public Toggle headBobToggle;
        public Toggle pedalToggle;

        public bool _isOpen = false;

        private void Start()
        {
            optionConfirmButton.onClick.AddListener(Close);
            optionResetButton.onClick.AddListener(OnOptionResetButtonClick);
            GlobalConfiguration.Instance.LoadAllProperty();
            InitializeOption();
            GlobalInputBinder.CreateGetKeyDownStream(KeyCode.Escape)
                .Where(_=> !_isOpen && StoryModeManager.GetState()==StoryModeManager.State.Walking)
                .Subscribe(_ => Open()).AddTo(gameObject);
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
            GlobalConfiguration.Instance.ResetOption();
            GlobalConfiguration.Instance.LoadAllProperty();
            InitializeOption();
        }

        private void Close()
        {
            SoundManager.PlayOneShot("ui_button", 1);
            optionPanel.gameObject.SetActive(false);
            PlayerPrefs.Save();
            Cursor.lockState = CursorLockMode.Locked;
            StoryModeManager.Instance.GetPlayer().SetCursorLocked();
            StoryModeManager.Instance.GetPlayer().SetMovable(true);
            GlobalConfiguration.Instance.SetFog(true);
            _isOpen = false;
        }

        private void Open()
        {
            SoundManager.PlayOneShot("ui_button", 1);
            optionPanel.gameObject.SetActive(true);
            GlobalConfiguration.Instance.SetFog(false);
            StoryModeManager.Instance.GetPlayer().SetCursorUnlocked();
            StoryModeManager.Instance.GetPlayer().SetMovable(false);
            _isOpen = true;
        }
    }
}
