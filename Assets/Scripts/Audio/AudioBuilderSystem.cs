using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public class AudioBuilderSystem : MonoBehaviour
    {
        public int maxStageSpotsAudioCache = 10;
        
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private AudioMixerGroup musicMixerGroup;

        private List<AudioClip> _builtClips;
        private CustomAudioSource _customAudioSource;

        private void Awake()
        {
            _builtClips = new List<AudioClip>(new AudioClip[maxStageSpotsAudioCache]);
            _customAudioSource = audioManager.Setup(musicMixerGroup, false);
        }
        
        public void UpdateClipAtIndex(AudioClip clip, int index)
        {
            clip?.LoadAudioData(); // Since clip can be null we use null prop
            _builtClips[index] = clip;
        }

        public void AddClipToBuilder(AudioClip clip)
        {
            clip.LoadAudioData();
            _builtClips.Add(clip);
        }

        public float PlayBuiltClips()
        {
            float longestClip = 0;
            foreach (var clip in _builtClips.Where(clip => clip is not null))
            {
                if (clip.length > longestClip) longestClip = clip.length;
                _customAudioSource.PlayOnce(clip);
            }
            
            return longestClip;
        }
    }
}