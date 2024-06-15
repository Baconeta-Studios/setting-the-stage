using UnityEngine;

namespace Audio
{
    public class PerformanceAudioController : MonoBehaviour
    {
        private AudioBuilderSystem _audioBuilder;
        private Chapter _chapter;
        private PerformanceAudioDataManager _audioDataManager;

        [SerializeField] private AudioClip testClip1;

        private void OnEnable()
        {
            StagePosition.OnStagePositionChanged += StagePositionChanged;
            _audioDataManager = FindObjectOfType<PerformanceAudioDataManager>();
            if (_audioDataManager is null)
            {
                StSDebug.LogError("Something went wrong - there is no PerformanceAudioDataManager in the scene.");
            }
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

        private void StagePositionChanged(StagePosition stagePosition)
        {
            if (stagePosition.instrumentOccupied is null || stagePosition.musicianOccupied is null)
            {
                return;
            }
            AudioClip clipToLoad = _audioDataManager.GetAudioTrack(_chapter.ChapterNumber, _chapter.ChapterNumber, stagePosition.instrumentOccupied, stagePosition.GetMusicianProficiency());
            _audioBuilder.UpdateClipAtIndex(clipToLoad, stagePosition.stagePositionNumber);
        }
    }
}