using Audio;
using UnityEngine;

public class ButtonAudio : MonoBehaviour
{
    [SerializeField] private string buttonAudioName;
    
    public void PlayButtonAudio()
    {
        AudioWrapper.Instance.PlaySoundVoid(buttonAudioName);
    }
}