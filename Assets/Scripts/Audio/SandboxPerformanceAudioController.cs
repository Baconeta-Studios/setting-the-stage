namespace Audio
{
    public class SandboxPerformanceAudioController : PerformanceAudioController
    {
        private SandboxAudioDataManager _sandboxAudioDataManager;

        protected override void OnEnable()
        {
            StagePosition.OnStagePositionCommitted += StagePositionUpdated;
            _sandboxAudioDataManager = FindObjectOfType<SandboxAudioDataManager>();
            if (_sandboxAudioDataManager is null)
            {
                StSDebug.LogError("Something went wrong - there is no SandboxAudioDataManager in the scene.");
            }
        }

        protected override void OnDisable()
        {
            StagePosition.OnStagePositionCommitted -= StagePositionUpdated;
        }

        protected override void Start()
        {
            _audioBuilder = FindObjectOfType<AudioBuilderSystem>();
            if (!_audioBuilder)
            {
                StSDebug.LogError($"SandboxPerformanceAudioController could not find audioBuilder object.");
            }
        }

        protected override void StagePositionUpdated(StagePosition stagePosition)
        {
            // What do we do when the stage position is updated
            // In this instance we have just changed an instrument - or we have started performing
            // So what we actually want to do is to let the UI dictate the exact track we load and when
            // For now, we do nothing else except clear a track if needed
            
            if (stagePosition.instrumentOccupied == null || stagePosition.musicianOccupied ==  null)
            {
                _audioBuilder.UpdateClipAtIndex(null, stagePosition.stagePositionNumber);
                return;
            }
            
        }
    }
}