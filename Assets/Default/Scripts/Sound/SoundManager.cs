using Default.Scripts.Util;
using Default.Scripts.Util.StatePattern;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Default.Scripts.Sound
{
    [CustomEditor(typeof(SoundManager))]
    public class SoundManagerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var sm = (SoundManager)target;
            sm.channels = sm.GetComponentsInChildren<Channel>();
        }
    }

    public class SoundManager : Singleton<SoundManager>
    {
        public SoundListAsset asset;
        [HideInInspector]
        public Channel[] channels;
        public static void Play(string name,int channel)
        {
            Instance.channels[channel].Play(Instance.asset.GetSoundByName(name).clip);
        }

        public static void Play(string name)
        {
            Instance.channels[0].Play(Instance.asset.GetSoundByName(name).clip);
        }

        public static void Stop(int channel)
        {
            Instance.channels[channel].Stop();
        }

        public static void Pause(int channel)
        {
            Instance.channels[channel].Pause();
        }

        public static void SetVolume(float volume,int channel)
        {
            Instance.channels[channel].SetVolume(volume);
        }
    }
}
