using System;
using UnityEngine;

namespace Default.Scripts.Sound
{
    [Serializable]
    public class Sound
    {
        public Sound(AudioClip clip)
        {
            this.clip = clip;
            name = clip.name;
        }
        public AudioClip clip;
        public string name;
    }
}