public class SandboxChapter : Chapter
{ 
    public override bool IsSandboxChapter => true;

    public override void EndChapterEarly()
    {
        StSDebug.Log($"Leaving sandbox mode.");
        ambient?.StopAudio();
        SceneLoader.Instance.LoadScene("MainMenu");
    }
}