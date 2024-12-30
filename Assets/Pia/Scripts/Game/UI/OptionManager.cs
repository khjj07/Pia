using Default.Scripts.Sound;
using Pia.Scripts.Manager;
using System;
using System.Globalization;
using Default.Scripts.Util;
using Pia.Scripts.StoryMode;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System.Security.Cryptography;

namespace Assets.Pia.Scripts.Game.UI
{
    public class OptionManager : MonoBehaviour
    {
 
        public Image optionPanel;
        public Button optionConfirmButton;
        public Button optionResetButton;

  
        public Slider mouseSensitiveSlider;
        public TMP_Text mouseSensitiveText;
        public Slider volumeSlider;
        public TMP_Text volumeText;
        public Toggle motionBlurToggle;
        public Toggle headBobToggle;
        public Toggle pedalToggle;


        public TMP_Dropdown screenModeDropdown;
        public TMP_Dropdown resolutionDropdown;
        public Slider frameLimitSlider;
        public TMP_Text frameLimitText;
        public Toggle verticalSyncToggle;

    

        public Button exposurePenalButton;
        public Button exposurePenalApplyButton;
        public Button exposurePenalResetButton;
        public GameObject exposureTest;
        public Slider exposureSlider;

        public bool _isOpen = false;

        public void Initialize()
        {
            optionConfirmButton.onClick.AddListener(Close);
            optionResetButton.onClick.AddListener(OnOptionResetButtonClick);
            exposurePenalButton.onClick.AddListener(OnExposurePanelButtonClick);
            exposurePenalResetButton.onClick.AddListener(OnExposurePanelResetButtonClick);
            exposurePenalApplyButton.onClick.AddListener(OnExposurePanelApplyButtonClick);
            GlobalConfiguration.Instance.LoadAllProperty();
            InitializeOption();
        }

        private void OnExposurePanelApplyButtonClick()
        {
            exposureTest.gameObject.SetActive(false);
            SoundManager.PlayOneShot("ui_button", 1);
        }

        private void OnExposurePanelResetButtonClick()
        {
            PlayerPrefs.DeleteKey("exposure");
            GlobalConfiguration.Instance.LoadAllProperty();
            SoundManager.PlayOneShot("ui_button", 1);
        }

        private void OnExposurePanelButtonClick()
        {
            exposureTest.gameObject.SetActive(true);
            SoundManager.PlayOneShot("ui_button", 1);
        }

        private void InitializeOption()
        {
            exposureSlider.value = GlobalConfiguration.Instance.GetExposure();
            motionBlurToggle.isOn = GlobalConfiguration.Instance.GetMotionBlur();
            headBobToggle.isOn = GlobalConfiguration.Instance.GetHeadBob();
            pedalToggle.isOn = GlobalConfiguration.Instance.GetPedalUse();

            var mouseSensitive = GlobalConfiguration.Instance.GetMouseSensitive();
            mouseSensitiveSlider.value = mouseSensitive;
            mouseSensitiveText.text = mouseSensitive.ToString("F1");

            var volume = GlobalConfiguration.Instance.GetVolume();
            volumeSlider.value = volume;
            volumeText.text = ((int)(volume * 100)).ToString();
            screenModeDropdown.value = GlobalConfiguration.Instance.GetScreenMode();
            var frameLimit = GlobalConfiguration.Instance.GetFrameLimit();
            frameLimitSlider.value = frameLimit;
            frameLimitText.text = frameLimit.ToString(CultureInfo.InvariantCulture);

            verticalSyncToggle.isOn = GlobalConfiguration.Instance.GetVsync();

            GlobalConfiguration.Instance.InitializeResolutionOptionData();
            InitializeResolutionDropDown();

            motionBlurToggle.onValueChanged.AddListener(GlobalConfiguration.Instance.SetMotionBlur);
            headBobToggle.onValueChanged.AddListener(GlobalConfiguration.Instance.SetHeadBob);
            pedalToggle.onValueChanged.AddListener(GlobalConfiguration.Instance.SetPedalUse);
            mouseSensitiveSlider.onValueChanged.AddListener(OnMouseSensitiveSliderChanged);
            screenModeDropdown.onValueChanged.AddListener(GlobalConfiguration.Instance.SetScreenMode);
            frameLimitSlider.onValueChanged.AddListener(OnFrameLimitChanged); 
            resolutionDropdown.onValueChanged.AddListener(GlobalConfiguration.Instance.SetResolution);
            volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);
            verticalSyncToggle.onValueChanged.AddListener(OnVsyncToggleChanged);
            exposureSlider.onValueChanged.AddListener(GlobalConfiguration.Instance.SetExposure);
        }

        private void InitializeResolutionDropDown()
        {
            resolutionDropdown.options.Clear();
            
            foreach (var item in GlobalConfiguration.Instance.GetResolutionList())
            {
                if (item.refreshRateRatio.value == 60.0f)
                {
                    TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
                    resolutionDropdown.options.Add(option);
                    option.text = item.width + " x " + item.height;
                }
            }
            resolutionDropdown.value = GlobalConfiguration.Instance.GetResolution();
        }

        private void OnFrameLimitChanged(float arg0)
        {
            GlobalConfiguration.Instance.SetFrameLimit(arg0);
            frameLimitText.text = arg0.ToString(CultureInfo.InvariantCulture);
        }

        private void OnMouseSensitiveSliderChanged(float arg0)
        {
            GlobalConfiguration.Instance.SetMouseSensitive(arg0);
            mouseSensitiveText.text = arg0.ToString("F1");
        }

        private void OnVolumeSliderChanged(float arg0)
        {
            GlobalConfiguration.Instance.SetVolume(arg0);
            int num = (int)(arg0*100);
            volumeText.text = num.ToString();
        }

        private void OnVsyncToggleChanged(bool arg0)
        {
            GlobalConfiguration.Instance.SetVsync(arg0);
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
            _isOpen = false;
        }

        public void Open()
        {
            SoundManager.PlayOneShot("ui_button", 1);
            optionPanel.gameObject.SetActive(true);
            GlobalConfiguration.Instance.SetFog(false);
            _isOpen = true;
        }
    }
}
