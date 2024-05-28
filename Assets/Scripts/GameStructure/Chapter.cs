using System;
using System.Collections.Generic;
using GameStructure;
using UnityEngine;
using Utils;

public class Chapter : Singleton<Chapter>
{
    public int ChapterNumber;
    public event Action<float> onChapterComplete;
    public event Action<ChapterStage> onStageChanged;

    // The amount of stars the player earned in the performance. -1 indicates the performance has not occured.
    public float starsEarned = -1;
    
    [Serializable]
    public enum ChapterStage
    {
        Intro,
        StageSelection,
        Performing,
        Ratings,
    }

    [SerializeField] private ChapterStage currentStage = ChapterStage.Intro;
    
    [SerializeField] private List<Musician> musicians = new List<Musician>();
    [SerializeField] private List<Instrument> instruments = new List<Instrument>();
    private List<Musician> availableMusicians = new List<Musician>();
    private List<Instrument> availableInstruments = new List<Instrument>();

    protected override void Awake()
    {
        base.Awake();
        musicians.Sort();
        instruments.Sort();
        availableMusicians = new List<Musician>(musicians);
        availableInstruments = new List<Instrument>(instruments);
    }

    private void OnEnable()
    {
        PerformanceController.OnPerformanceEnded += OnPerformanceComplete;
    }

    private void OnDisable()
    {
        PerformanceController.OnPerformanceEnded -= OnPerformanceComplete;
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
        onChapterComplete?.Invoke(starsEarned);
    }

    public ChapterStage GetCurrentStage()
    {
        return currentStage;
    }

    public bool IsInCurrentStage(ChapterStage stageToCheck)
    {
        return currentStage == stageToCheck;
    }

    public List<Musician> GetAvailableMusicians()
    {
        return availableMusicians;
    }

    public List<Instrument> GetAvailableInstruments()
    {
        return availableInstruments;
    }

    public bool ConsumeMusician(Musician musician)
    {
        if (availableMusicians.Contains(musician))
        {
            availableMusicians.Remove(musician);
            return true;
        }

        return false;
    }

    public void ReturnMusician(Musician musician)
    {
        if (musician is not null && !availableMusicians.Contains(musician))
        {
            availableMusicians.Add(musician);
            availableMusicians.Sort();
        }
    }

    public bool ConsumeInstrument(Instrument instrument)
    {
        if (availableInstruments.Contains(instrument))
        {
            availableInstruments.Remove(instrument);
            return true;
        }

        return false;
    }

    public void ReturnInstrument(Instrument instrument)
    {
        if (instrument is not null && !availableInstruments.Contains(instrument))
        {
            availableInstruments.Add(instrument);
            availableInstruments.Sort();
        }
    }

    public void ReturnObject(StSObject returningObject)
    {
        if (returningObject is null)
        {
            return;
        }
        
        foreach (Musician musician in musicians)
        {
            if (musician.GetName() == returningObject.GetName())
            {
                ReturnMusician((Musician)returningObject);
                return;
            }
        }

        foreach (Instrument instrument in instruments)
        {
            if (instrument.GetName() == returningObject.GetName())
            {
                ReturnInstrument((Instrument)returningObject);
                return;
            }
        }
        
        StSDebug.LogError($"{gameObject.name}{ChapterNumber}: Could not find musician or instrument when returning an object. Something has gone wrong :(");
    }

    public bool ConsumeObject(StSObject objectToConsume)
    {
        if (objectToConsume is null)
        {
            return false;
        }
        
        foreach (Musician musician in musicians)
        {
            if (musician.GetName() == objectToConsume.GetName())
            {
                return ConsumeMusician((Musician)objectToConsume);
            }
        }

        foreach (Instrument instrument in instruments)
        {
            if (instrument.GetName() == objectToConsume.GetName())
            {
                return ConsumeInstrument((Instrument)objectToConsume);
            }
        }
        
        StSDebug.LogError($"{gameObject.name}{ChapterNumber}: Could not find musician or instrument when returning an object. Something has gone wrong :(");
        return false;
    }

    void OnPerformanceComplete(float newStarsEarned)
    {
        starsEarned = newStarsEarned;
        
        NextStage();
    }
}
