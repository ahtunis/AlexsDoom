using UnityEngine;

namespace AlexsDoom.Level
{
    /// <summary>
    /// Singleton music player. Persists across scenes.
    /// Call AudioManager.Instance.PlayTrack(index) from any scene-load code.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioClip[] musicTracks;
        [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.6f;

        private AudioSource _musicSource;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _musicSource = GetComponent<AudioSource>();
            _musicSource.loop = true;
            _musicSource.volume = musicVolume;
        }

        public void PlayTrack(int index)
        {
            if (musicTracks == null || index < 0 || index >= musicTracks.Length) return;
            _musicSource.clip = musicTracks[index];
            _musicSource.Play();
        }

        public void PlayTrack(AudioClip clip)
        {
            if (clip == null) return;
            _musicSource.clip = clip;
            _musicSource.Play();
        }

        public void SetVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            _musicSource.volume = musicVolume;
        }

        public void Stop() => _musicSource.Stop();

        public void Pause() => _musicSource.Pause();

        public void Resume() => _musicSource.UnPause();
    }
}
