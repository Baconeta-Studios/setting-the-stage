namespace Audio
{
    public class SandboxPerformanceAudioController : PerformanceAudioController
    {
        private SandboxAudioDataManager _sandboxAudioDataManager;

        protected override void OnEnable()
        {
            StagePosition.OnStagePositionCommitted += StagePositionUpdated;
            SandboxMusicianPanel.OnTrackSelected += UpdateAudioOnTrackSelection;
            
            _sandboxAudioDataManager = FindObjectOfType<SandboxAudioDataManager>();
            if (_sandboxAudioDataManager == null)
            {
                StSDebug.LogError("Something went wrong - there is no SandboxAudioDataManager in the scene.");
                _sandboxAudioDataManager = SandboxAudioDataManager.Instance;
            }
        }

        protected override void OnDisable()
        {
            StagePosition.OnStagePositionCommitted -= StagePositionUpdated;
            SandboxMusicianPanel.OnTrackSelected -= UpdateAudioOnTrackSelection;
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
            if (stagePosition.instrumentOccupied == null || stagePosition.musicianOccupied ==  null)
            {
                _audioBuilder.UpdateClipAtIndex(null, stagePosition.stagePositionNumber);
                return;
            }

            var sandboxPosition = stagePosition as SandboxStagePosition;
            if (sandboxPosition == null)
            {
                StSDebug.LogError("Something went wrong - there is no SandboxStagePosition");
                return;
            }
            UpdateAudioOnTrackSelection(sandboxPosition.selectedTrackName, sandboxPosition);
        }

        private void UpdateAudioOnTrackSelection(string trackName, SandboxStagePosition stagePosition)
        {
            if (stagePosition.musicianOccupied == null || stagePosition.instrumentOccupied == null || trackName == "" || trackName == null)
            {
                // we have to verify trackname exists also since many different scenarios can lead us down this code path
                return;
            }
            var trackClip = _sandboxAudioDataManager.GetAudioTrack(stagePosition.instrumentOccupied, trackName, stagePosition.GetMusicianProficiency());
            _audioBuilder.UpdateClipAtIndex(trackClip, stagePosition.stagePositionNumber);
        }
    }
}