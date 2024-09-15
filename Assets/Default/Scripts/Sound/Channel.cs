using UnityEngine;

namespace Default.Scripts.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class Channel : MonoBehaviour
    {
        [Range(0.0f, 1.0f)]
        public float volume = 0.5f;
        private AudioSource _audioSource;

        public void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Play(AudioClip clip)
        {
            _audioSource.clip = clip;
            _audioSource.volume = volume;
            _audioSource.Play();
        }
        public void PlayOneShot(AudioClip clip)
        {
            _audioSource.volume = volume;
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