using Utils;

namespace Audio
{
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

        public void RestartMenuAudio()
        {
            if (_backgroundMusic != null)
            {
                Destroy(_backgroundMusic.gameObject);
            }
            _backgroundMusic = AudioWrapper.Instance.PlaySound(backgroundMusicTrack);
            _backgroundMusic.transform.parent = gameObject.transform;
        }

        private void StartBackgroundMusic()
        {
            _backgroundMusic = AudioWrapper.Instance.PlaySound(backgroundMusicTrack);
            _backgroundMusic.transform.parent = gameObject.transform;
        }
    }
}