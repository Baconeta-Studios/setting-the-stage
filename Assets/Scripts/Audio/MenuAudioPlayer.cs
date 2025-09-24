using UnityEngine;

namespace Audio
{
    public class MenuAudioPlayer : MonoBehaviour
    {
        public void StopMenuAudio()
        {
            MainMenuAudio.Instance.StopMenuAudio();
        }

        public void RestartMenuAudio()
        {
            MainMenuAudio.Instance.RestartMenuAudio();
        }

        private void StartBackgroundMusic()
        {
            MainMenuAudio.Instance.StartBackgroundMusic();
        }
    }
}