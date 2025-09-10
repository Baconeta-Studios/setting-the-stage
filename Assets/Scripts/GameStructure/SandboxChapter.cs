public class SandboxChapter : Chapter
{ 
    public override bool IsSandboxChapter => true;

    public override void CompleteChapter()
    {
        StSDebug.Log($"Completed Sandbox chapter");
        ambient?.StopAudio();
        // MainMenuAudio.Instance.RestartMenuAudio();
        SceneLoader.Instance.LoadScene("Main Menu");
    }

    public override void EndChapterEarly()
    {
        StSDebug.Log($"Leaving sandbox mode.");
        ambient?.StopAudio();
        SceneLoader.Instance.LoadScene("Main Menu");
    }

    protected override void SaveLearnedProficiencies()
    {
        // Do nothing - we don't save knowledge learned in Sandbox mode
    }
}