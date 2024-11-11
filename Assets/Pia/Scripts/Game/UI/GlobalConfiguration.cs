using Default.Scripts.Sound;
using Default.Scripts.Util;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using static UnityEditor.AddressableAssets.Build.Layout.BuildLayout;
using static UnityEngine.Rendering.DebugUI;

namespace Assets.Pia.Scripts.Game.UI
{
    public class GlobalConfiguration : Singleton<GlobalConfiguration>
    {
        [SerializeField] private Volume graphciVolume;
        [SerializeField] private int motionBlur;
        [SerializeField] private int headBob;
        [SerializeField] private float mouseSensitive;
        [SerializeField] private float soundVolume;
        [SerializeField] private float exposure;


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
            SetExposure(GetFloatProperty("exposure", 0.75f));
            SetVolume(GetFloatProperty("soundVolume", 1));
            SetMouseSensitive(GetFloatProperty("mouseSensitive", 1));
            SetHeadBob(GetIntProperty("headBob", 1));
            SetMotionBlur(GetIntProperty("motionBlur", 1));

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

        public void SetMotionBlur(int value)
        {
            VolumeProfile profile = graphciVolume.sharedProfile;
            if (profile.TryGet<MotionBlur>(out var m))
            {
                m.active = value == 1 ? true : false;
                PlayerPrefs.SetInt("motionBlur", value);
                motionBlur = value;
            }
        }

        public int GetMotionBlur()
        {
            return headBob;
        }

        public void SetHeadBob(int value)
        {
            PlayerPrefs.SetFloat("headBob", value);
            headBob = value;
        }

        public int GetHeadBob()
        {
            return headBob;
        }
    }
}
