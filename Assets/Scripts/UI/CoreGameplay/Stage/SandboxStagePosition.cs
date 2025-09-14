using Audio;

/// This will allow us to have special logic like selecting a music track from the
/// UI and telling the audio manager which track we selected to add it to the audiobuilder <inheritdoc />
public class SandboxStagePosition : StagePosition
{
    public SandboxAudioDataManager sandboxAudioDataManager;
}