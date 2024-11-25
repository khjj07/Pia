using Default.Scripts.Sound;
using Default.Scripts.Util;
using UnityEngine;
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
        private float mouseSensitive;
        private float soundVolume;
        private float exposure;


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
            SetPedalUse(GetIntProperty("pedalUse", 1) == 1);
            SetMotionBlur(GetIntProperty("motionBlur", 1) == 1);
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
            Debug.Log(value);
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
    }
}
