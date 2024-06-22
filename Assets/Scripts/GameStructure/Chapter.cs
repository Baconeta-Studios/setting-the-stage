using System;
using System.Collections.Generic;
using GameStructure;
using Managers;
using UnityEngine;
using Utils;

public class Chapter : Singleton<Chapter>
{
    public int ChapterNumber;
    public event Action<float> onChapterComplete;
    public event Action<ChapterStage> onStageChanged;
    public event Action<float> onRevealRating;

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

    [SerializeField] private Transform StsObjectStash;
    private List<GameObject> musicians = new List<GameObject>();
    private List<GameObject> instruments = new List<GameObject>();
    private List<Musician> availableMusicians = new List<Musician>();
    private List<Instrument> availableInstruments = new List<Instrument>();

    protected override void Awake()
    {
        base.Awake();
        
        ChapterCarouselOptions carouselOptions = FindObjectOfType<ChapterCarouselOptions>();
        if (carouselOptions == null)
        {
            StSDebug.LogError("There's no carousel options in this chapter scene!");
        }
        musicians = carouselOptions.musicians;
        instruments = carouselOptions.instruments;
        
        musicians.Sort((a, b) => String.Compare(a.name, b.name, StringComparison.Ordinal));
        instruments.Sort((a, b) => String.Compare(a.name, b.name, StringComparison.Ordinal));
        availableMusicians = new List<Musician>();
        availableInstruments = new List<Instrument>();
        
        // Spawn all musicians into the stash.
        for (int i = 0; i < musicians.Count; i++)
        {
            GameObject musicianGameObject = Instantiate(musicians[i], StsObjectStash);
            
            musicianGameObject.SetActive(false);
            
            Musician musician = musicianGameObject.GetComponent<Musician>();
            if (!musician)
            {
                StSDebug.LogError($"No Class<Musician> found on {musicianGameObject.name}");
            }
            else
            {
                availableMusicians.Add(musician);
            }

            // Replace the prefab with the spawned object.
            musicians[i] = musicianGameObject;
        }
        
        // Spawn all Instruments into the stash.
        for (int i = 0; i < instruments.Count; i++)
        {
            GameObject instrumentGameObject = Instantiate(instruments[i], StsObjectStash);
            
            Instrument instrument = instrumentGameObject.GetComponent<Instrument>();
            if (!instrument)
            {
                StSDebug.LogError($"No Class<Instrument> found on {instrumentGameObject.name}");
            }
            else
            {
                availableInstruments.Add(instrument);
            }

            // Replace the prefab with the spawned object.
            instruments[i] = instrumentGameObject;
        }
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
    
    public void EndChapterEarly()
    {
        StSDebug.Log($"End Chapter {ChapterNumber} early");
        onChapterComplete?.Invoke(0);
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

    public GameObject ConsumeMusician(Musician musician)
    {
        if (availableMusicians.Contains(musician))
        {
            availableMusicians.Remove(musician);
            return musician.gameObject;
        }

        return null;
    }

    public void ReturnMusician(Musician musician)
    {
        if (musician != null && !availableMusicians.Contains(musician))
        {
            availableMusicians.Add(musician);
            availableMusicians.Sort();
        }
    }

    public GameObject ConsumeInstrument(Instrument instrument)
    {
        if (availableInstruments.Contains(instrument))
        {
            availableInstruments.Remove(instrument);
            return instrument.gameObject;
        }

        return null;
    }

    public void ReturnInstrument(Instrument instrument)
    {
        if (instrument != null && !availableInstruments.Contains(instrument))
        {
            availableInstruments.Add(instrument);
            availableInstruments.Sort();
        }
    }

    public void ReturnObject(StSObject returningObject)
    {
        if (returningObject == null)
        {
            return;
        }
        
        foreach (GameObject musician in musicians)
        {
            if (musician.GetComponent<Musician>().GetName() == returningObject.GetName())
            {
                ReturnMusician((Musician)returningObject);
                StashObject(returningObject);
                return;
            }
        }

        foreach (GameObject instrument in instruments)
        {
            if (instrument.GetComponent<Instrument>().GetName() == returningObject.GetName())
            {
                ReturnInstrument((Instrument)returningObject);
                StashObject(returningObject);
                return;
            }
        }
        
        StSDebug.LogError($"{gameObject.name}{ChapterNumber}: Could not find musician or instrument when returning an object. Something has gone wrong :(");
    }

    private void StashObject(StSObject objectToStash)
    {
        Transform objectTransform = objectToStash.transform;
        objectTransform.SetParent(StsObjectStash);
        objectTransform.localPosition = Vector3.zero;
        
        objectToStash.gameObject.SetActive(false);
    }

    public bool ConsumeObject(StSObject objectToConsume)
    {
        if (objectToConsume == null)
        {
            return false;
        }
        
        foreach (GameObject musician in musicians)
        {
            if (musician.GetComponent<Musician>().GetName() == objectToConsume.GetName())
            {
                return ConsumeMusician((Musician)objectToConsume);
            }
        }

        foreach (GameObject instrument in instruments)
        {
            if (instrument.GetComponent<Instrument>().GetName() == objectToConsume.GetName())
            {
                return ConsumeInstrument((Instrument)objectToConsume);
            }
        }
        
        StSDebug.LogError($"{gameObject.name}{ChapterNumber}: Could not find musician or instrument when returning an object. Something has gone wrong :(");
        return false;
    }

    private void OnPerformanceComplete(float newStarsEarned)
    {
        starsEarned = newStarsEarned;
        
        // TODO Move reveal rating to when we actually want to reveal the star count. Currently just enables the star UI above the chapter navigation.
        onRevealRating?.Invoke(starsEarned);
        
        NextStage();
    }
}
