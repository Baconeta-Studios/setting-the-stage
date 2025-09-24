using Utils;

namespace Audio
{
    /// <summary>
    /// This script persists and also holds the audio object playing the bg music
    /// </summary>
    public class MainMenuAudio : EverlastingSingleton<MainMenuAudio>
    {
        
        public string backgroundMusicTrack;
        private CustomAudioSource _backgroundMusic;

        public void Start()
        {
            if (_backgroundMusic == null)
            {
                Invoke(nameof(StartBackgroundMusic), 0.25f);
            }
        }
        
        public void StopMenuAudio()
        {
            if (_backgroundMusic != null)
            {
                _backgroundMusic.StopAudio();
            }
        }

        public void RestartMenuAudio()
        {
            if (_backgroundMusic != null)
            {
                Destroy(_backgroundMusic.gameObject);
            }
            _backgroundMusic = AudioWrapper.Instance.PlaySound(backgroundMusicTrack);
            _backgroundMusic.transform.parent = gameObject.transform;
        }

        public void StartBackgroundMusic()
        {
            _backgroundMusic = AudioWrapper.Instance.PlaySound(backgroundMusicTrack);
            _backgroundMusic.transform.parent = gameObject.transform;
        }
    }
}