using System;
using UnityEngine;
using Utils;

public class Chapter : Singleton<Chapter>
{
    public int ChapterNumber;
    public event Action onChapterComplete;
    public event Action<ChapterStage> onStageChanged;
    
    [Serializable]
    public enum ChapterStage
    {
        Intro,
        StageSelection,
        Performing,
        Ratings,
    }

    [SerializeField] private ChapterStage currentStage = ChapterStage.Intro;

    private void Start()
    {
        // Chapter has just been opened. Start the intro cutscene.
        // TODO Create intro cutscene
        StSDebug.Log($"Started Chapter {ChapterNumber}");
        onStageChanged?.Invoke(currentStage);
    }
    
    public void NextStage()
    {
        StSDebug.Log($"Finished Chapter {ChapterNumber}: {currentStage.ToString()}");

        switch (currentStage)
        {
            case ChapterStage.Intro:
                currentStage = ChapterStage.StageSelection;
                break;
            case ChapterStage.StageSelection:
                currentStage = ChapterStage.Performing;
                break;
            case ChapterStage.Performing:
                currentStage = ChapterStage.Ratings;
                break;
            case ChapterStage.Ratings:
                CompleteChapter();
                return;
        }
        
        StSDebug.Log($"Starting Chapter {ChapterNumber}: {currentStage.ToString()}");
        onStageChanged?.Invoke(currentStage);
    }
    
    public void CompleteChapter()
    {
        StSDebug.Log($"Completed Chapter {ChapterNumber}");
        onChapterComplete?.Invoke();
    }

    public ChapterStage GetCurrentStage()
    {
        return currentStage;
    }

    public bool IsInCurrentStage(ChapterStage stageToCheck)
    {
        return currentStage == stageToCheck;
    }
}
