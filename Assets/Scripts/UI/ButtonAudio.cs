using Audio;
using UnityEngine;

public class ButtonAudio : MonoBehaviour
{
    [SerializeField] private string selectButtonAudioName;
    [SerializeField] private string cancelButtonAudioName;
    [SerializeField] private string simpleButtonAudioName;
    
    public void PlaySelectButtonAudio()
    {
        AudioWrapper.Instance.PlaySound(selectButtonAudioName);
    }
    
    public void PlayCancelButtonAudio()
    {
        AudioWrapper.Instance.PlaySound(cancelButtonAudioName);
    }
    
    public void PlaySimpleButtonAudio()
    {
        AudioWrapper.Instance.PlaySound(simpleButtonAudioName);
    }
}