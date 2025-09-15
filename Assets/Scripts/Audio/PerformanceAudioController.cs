using UnityEngine;

namespace Audio
{
    public class PerformanceAudioController : MonoBehaviour
    {
        protected AudioBuilderSystem _audioBuilder;
        private Chapter _chapter;
        private PerformanceAudioDataManager _audioDataManager;
        private Act _act;

        [SerializeField] private AudioClip testClip1;

        protected virtual void OnEnable()
        {
            StagePosition.OnStagePositionCommitted += StagePositionUpdated;
            _audioDataManager = FindObjectOfType<PerformanceAudioDataManager>();
            if (_audioDataManager is null)
            {
                StSDebug.LogError("Something went wrong - there is no PerformanceAudioDataManager in the scene.");
            }
        }

        protected virtual void OnDisable()
        {
            StagePosition.OnStagePositionCommitted -= StagePositionUpdated;
        }

        protected virtual void Start()
        {
            _chapter = FindObjectOfType<Chapter>();
            if (!_chapter)
            {
                StSDebug.LogError($"PerformanceAudioController could not find chapter object.");
            }
            
            _act = FindObjectOfType<Act>();
            if (!_act && !_chapter.IsSandboxChapter) // The sandbox chapter does not need to worry about performance (todo revisit)
            {
                StSDebug.LogError($"PerformanceAudioController could not find act object.");
            }
            
            _audioBuilder = FindObjectOfType<AudioBuilderSystem>();
            if (!_audioBuilder)
            {
                StSDebug.LogError($"PerformanceAudioController could not find audioBuilder object.");
            }
        }

        protected virtual void StagePositionUpdated(StagePosition stagePosition)
        {
            if (stagePosition.instrumentOccupied == null || stagePosition.musicianOccupied ==  null)
            {
                _audioBuilder.UpdateClipAtIndex(null, stagePosition.stagePositionNumber);
                return;
            }

            AudioClip clipToLoad = _audioDataManager.GetAudioTrack(_act.GetActNumber(), _chapter.ChapterNumber, stagePosition.instrumentOccupied, stagePosition.GetMusicianProficiency());
            _audioBuilder.UpdateClipAtIndex(clipToLoad, stagePosition.stagePositionNumber);
        }
    }
}