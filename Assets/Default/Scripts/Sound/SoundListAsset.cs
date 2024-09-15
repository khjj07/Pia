using Default.Scripts.Extension;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Default.Scripts.Sound
{
#if UNITY_EDITOR
    [CustomEditor(typeof(SoundListAsset))]
    public class SoundListAssetInspector : Editor
    {
       

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var sl = (SoundListAsset)target;
            if (GUILayout.Button("Load All Sound"))
            {
                var clips = sl.floder.LoadAllObjectsInFolder<AudioClip>();
                sl.sounds = new List<Sound>();
                foreach (var clip in clips)
                {
                    sl.sounds.Add(new Sound(clip));
                }
            }
        }
    }
#endif
    [CreateAssetMenu(fileName = "Sound List Asset",menuName = "Sound/Sound LIst Asset")]
    public class SoundListAsset : ScriptableObject
    {
        public List<Sound> sounds;
        public DefaultAsset floder;
        public Sound GetSoundByName(string name)
        {
            var soundsQuery = from sound in sounds
                where sound.name == name
                select sound;
            foreach (var sound in soundsQuery)
            {
                return sound;
            }

            return null;
        }
    }
}