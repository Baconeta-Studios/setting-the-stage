using UnityEngine;

namespace Audio
{
    public class PerformanceAudioController : MonoBehaviour
    {
        private AudioBuilderSystem _audioBuilder;
        private Chapter _chapter;

        [SerializeField] private AudioClip testClip1;

        private void OnEnable()
        {
            StagePosition.OnStagePositionChanged += StagePositionChanged;
        }

        private void OnDisable()
        {
            StagePosition.OnStagePositionChanged -= StagePositionChanged;
        }

        private void Start()
        {
            _chapter = FindObjectOfType<Chapter>();
            if (!_chapter)
            {
                StSDebug.LogError($"PerformanceAudioController could not find chapter object.");
            }
            
            _audioBuilder = FindObjectOfType<AudioBuilderSystem>();
            if (!_audioBuilder)
            {
                StSDebug.LogError($"PerformanceAudioController could not find audioBuilder object.");
            }
        }

        private void StagePositionChanged(StagePosition obj)
        {
            // Here we will need to get the chapter data which includes everything we need to know about which audio tracks to add or not
            // based on the selection in StagePosition
            
            // get chapter data from _chapter and musician proficiency from the stagePosition. 
            // TODO replace testClip with actual clip to play
            _audioBuilder.UpdateClipAtIndex(testClip1, obj.stagePositionNumber);
        }
    }
}