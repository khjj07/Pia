using Default.Scripts.Sound;
using Default.Scripts.Util;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Assets.Pia.Scripts.Game.UI
{
    public class GlobalConfiguration : Singleton<GlobalConfiguration>
    {
        [SerializeField] private Volume graphciVolume;
        public bool motionBlur;
        public bool headBob;
        public bool isPedalUse;
        public bool vsync;
        public int frameLimit;
        public int screenMode;
        private float mouseSensitive;
        private float soundVolume;
        private float exposure;
        public int language;
        public int resolution;

        private List<Resolution> resolutionList = new List<Resolution>();

        public float GetFloatProperty(string name, float alt = 0f)
        {
            if (PlayerPrefs.HasKey(name))
            {
                return PlayerPrefs.GetFloat(name);
            }
            return alt;
        }
        public int GetIntProperty(string name, int alt = 0)
        {
            if (PlayerPrefs.HasKey(name))
            {
                return PlayerPrefs.GetInt(name);
            }
            return alt;
        }
        public string GetStringProperty(string name, string alt)
        {
            if (PlayerPrefs.HasKey(name))
            {
                return PlayerPrefs.GetString(name);
            }
            return alt;
        }

      
        public void Start()
        {
            Cursor.visible = false;
            LoadAllProperty();
        }
        public void LoadAllProperty()
        {
            SetExposure(GetFloatProperty("exposure", 0.4f));
            SetVolume(GetFloatProperty("soundVolume", 1));
            SetMouseSensitive(GetFloatProperty("mouseSensitive", 1));
            SetHeadBob(GetIntProperty("headBob", 1) == 1);
            SetVsync(GetIntProperty("vsync", 1) == 1);
            SetPedalUse(GetIntProperty("pedalUse", 0) == 1);
            SetFrameLimit(GetIntProperty("frameLimit", 30));
            SetMotionBlur(GetIntProperty("motionBlur",1) == 1);
            SetLanguage(GetIntProperty("language", 0));
            SetScreenMode(GetIntProperty("screenMode", 0));
            InitializeResolutionOptionData();
        }


        public void ResetOption()
        {
            PlayerPrefs.DeleteKey("exposure");
            PlayerPrefs.DeleteKey("soundVolume");
            PlayerPrefs.DeleteKey("mouseSensitive");
            PlayerPrefs.DeleteKey("headBob");
            PlayerPrefs.DeleteKey("pedalUse");
            PlayerPrefs.DeleteKey("motionBlur");
            PlayerPrefs.DeleteKey("vsync");
            PlayerPrefs.DeleteKey("screenMode");
        }
        private string LanguageToString(int lang)
        {
            switch (lang)
            {
                case 0:
                    return "en";
                case 1:
                    return "ko";
                case 2:
                    return "ja";
                default:
                   return "en";
            }
        }

        public void SetFog(bool value)
        {
            VolumeProfile profile = graphciVolume.sharedProfile;

            if (profile.TryGet<Fog>(out var fog))
            {
                fog.enabled.value = value;
            }
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public void SetExposure(float value)
        {
            VolumeProfile profile = graphciVolume.sharedProfile;

            if (profile.TryGet<Exposure>(out var exp))
            {
                exp.fixedExposure.value = value;
                PlayerPrefs.SetFloat("exposure", value);
                exposure = value;
            }
        }
        public void SetPedalUse(bool value)
        {
            isPedalUse = value;
            PlayerPrefs.SetInt("pedalUse", value ? 1 : 0);
            Debug.Log(isPedalUse);
        }
        public void SetVsync(bool b)
        {
            vsync = b;
            PlayerPrefs.SetInt("vsync", vsync ? 1 : 0);
            QualitySettings.vSyncCount = vsync ? 1 : 0;
        }

        public void SetFrameLimit(float frame)
        {
            frameLimit = (int)frame;
            PlayerPrefs.SetInt("frameLimit", frameLimit);
            Application.targetFrameRate = 30;
        }

        public void SetScreenMode(int mode)
        {
            screenMode = mode;
            PlayerPrefs.SetInt("screenMode", screenMode);
            Screen.fullScreenMode = IntegerToScreenMode(screenMode);
        }

        public FullScreenMode IntegerToScreenMode(int mode)
        {
            switch (screenMode)
            {
                case 0:
                    return FullScreenMode.Windowed;
                    break;
                case 1:
                    return FullScreenMode.ExclusiveFullScreen;
                    
                    break;
                case 2:
                    return FullScreenMode.FullScreenWindow;
                    break;
                default:
                    return FullScreenMode.Windowed;
            }
        }

        public int GetScreenMode()
        {
            return screenMode;
        }

        public int GetFrameLimit()
        {
            return frameLimit;
        }

        public bool GetVsync()
        {
            return vsync;
        }

        public bool GetPedalUse()
        {
            return isPedalUse;
        }

        public float GetExposure()
        {
            return exposure;
        }

        public void SetVolume(float value)
        {
            SoundManager.SetMainVolume(value);
            PlayerPrefs.SetFloat("soundVolume", value);
            soundVolume = value;
        }

        public float GetVolume()
        {
            return soundVolume;
        }

        public void SetMouseSensitive(float value)
        {
            PlayerPrefs.SetFloat("mouseSensitive", value);
            mouseSensitive = value;
        }

        public float GetMouseSensitive()
        {
            return mouseSensitive;
        }

        public void SetMotionBlur(bool value)
        {
            motionBlur = value;
            VolumeProfile profile = graphciVolume.sharedProfile;
            if (profile.TryGet<MotionBlur>(out var m))
            {
                m.active = value;
                PlayerPrefs.SetInt("motionBlur", value ? 1 : 0);
            }
        }
        public bool GetMotionBlur()
        {
            return motionBlur;
        }
        public void SetHeadBob(bool value)
        {
            headBob = value;
            PlayerPrefs.SetInt("headBob", value ? 1 : 0);
        } 
        public bool GetHeadBob()
        {
            return headBob;
        }
        public int GetLanguage()
        {
            return language;
        }
        public void SetLanguage(int value)
        {
            LocaleIdentifier localeCode = new LocaleIdentifier(LanguageToString(value));
            for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
            {
                Locale aLocale = LocalizationSettings.AvailableLocales.Locales[i];
                LocaleIdentifier anIdentifier = aLocale.Identifier;
                if (anIdentifier == localeCode)
                {
                    LocalizationSettings.SelectedLocale = aLocale;
                }
            }

            language = value;
            PlayerPrefs.SetInt("language", value);
        }

        public int GetResolution()
        {
            return resolution;
        }
        public void SetResolution(int num)
        {
            resolution = num;
            Screen.SetResolution(resolutionList[resolution].width, resolutionList[resolution].height, IntegerToScreenMode(screenMode));
        }

        public List<Resolution> GetResolutionList()
        {
            return resolutionList;
        }

        public void InitializeResolutionOptionData()
        {

            resolutionList=new List<Resolution>();
            int index = 0;
            foreach (Resolution item in Screen.resolutions)
            {
                if (item.refreshRateRatio.value == 60.0f)
                {
                    resolutionList.Add(item);
                    if (item.width == Screen.width && item.height == Screen.height)
                    {
                        SetResolution(index);
                    }
                    index++;
                }
            }
        }
    }
}
