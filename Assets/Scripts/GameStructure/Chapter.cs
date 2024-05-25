using System;
using System.Collections;
using System.Collections.Generic;
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
    
    [SerializeField] private List<string> musicians = new List<string>();
    [SerializeField] private List<string> instruments = new List<string>();
    private List<string> availableMusicians = new List<string>();
    private List<string> availableInstruments = new List<string>();

    protected override void Awake()
    {
        base.Awake();
        musicians.Sort();
        instruments.Sort();
        availableMusicians = musicians;
        availableInstruments = instruments;
    }
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
                StartCoroutine(Perform());
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

    IEnumerator Perform()
    {
        yield return new WaitForSeconds(3f);
        NextStage();
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

    public List<string> GetAvailableMusicians()
    {
        return availableMusicians;
    }

    public List<string> GetAvailableInstruments()
    {
        return availableInstruments;
    }

    public bool ConsumeMusician(string musician)
    {
        if (availableMusicians.Contains(musician))
        {
            availableMusicians.Remove(musician);
            return true;
        }

        return false;
    }

    public void ReturnMusician(string musician)
    {
        if (musician != string.Empty && !availableMusicians.Contains(musician))
        {
            availableMusicians.Add(musician);
            availableMusicians.Sort();
        }
    }

    public bool ConsumeInstrument(string instrument)
    {
        if (availableInstruments.Contains(instrument))
        {
            availableInstruments.Remove(instrument);
            return true;
        }

        return false;
    }

    public void ReturnInstrument(string instrument)
    {
        if (instrument != string.Empty && !availableInstruments.Contains(instrument))
        {
            availableInstruments.Add(instrument);
            availableInstruments.Sort();
        }
    }
}
