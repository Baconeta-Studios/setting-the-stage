namespace Audio
{
    public class SandboxPerformanceAudioController : PerformanceAudioController
    {
        private SandboxAudioDataManager _sandboxAudioDataManager;

        protected override void OnEnable()
        {
            StagePosition.OnStagePositionCommitted += StagePositionUpdated;
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
            
            // TODO make sure this is called when we swap instruments - possibly we could consider this being the default
            //game call and then from there we manually make a UI call to the sandbox controller from the special UI
            // TODO also build special UI for inside sandbox mode to handle this scenario which will
            //also need to have access to _sandboxAudioDataManager to be able to use GetAllTracksForInstrument for UI
            
            if (stagePosition.instrumentOccupied == null || stagePosition.musicianOccupied ==  null)
            {
                _audioBuilder.UpdateClipAtIndex(null, stagePosition.stagePositionNumber);
            }
        }

        public void UpdateAudioOnTrackSelection(string trackName, StagePosition stagePosition)
        {
            //TODO make this be called from UI changes (selecting a track)
            var trackClip = _sandboxAudioDataManager.GetAudioTrack(stagePosition.instrumentOccupied, trackName, stagePosition.GetMusicianProficiency());
            _audioBuilder.UpdateClipAtIndex(trackClip, stagePosition.stagePositionNumber);
        }
    }
}