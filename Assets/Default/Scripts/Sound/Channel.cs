using UnityEngine;

namespace Default.Scripts.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class Channel : MonoBehaviour
    {
        [Range(0.0f, 1.0f)]
        public float volume = 0.5f;
        public bool loop = false;
        private AudioSource _audioSource;

        public void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            volume=_audioSource.volume;
            loop=_audioSource.loop;
        }

        public void Play(AudioClip clip)
        {
            _audioSource.clip = clip;
            _audioSource.volume = volume * SoundManager.GetMainVolume();
            _audioSource.Play();
        }        public void PlayOneShot(AudioClip clip)
        {
            _audioSource.volume = volume * SoundManager.GetMainVolume();
            _audioSource.PlayOneShot(clip);
        }

        public void Stop()
        {
            _audioSource.Stop();
        }

        public void Pause()
        {
            _audioSource.Pause();
        }

        public void SetVolume(float v)
        {
            volume = v;
            _audioSource.volume = volume;
        }
    }
}