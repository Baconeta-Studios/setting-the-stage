using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameStructure;
using UnityEngine;
using Utils;

[Serializable]
public class ChapterStruct
{
    public float starsEarned;
    public float starsRequiredToUnlock;
    public SceneStruct sceneInfo;
}

public class Act : MonoBehaviour
{
    [SerializeField] private int actNumber;
    
    [Header("Chapters")]
    [SerializeField] private List<ChapterStruct> chapters;
    
    [SerializeField] private int currentChapterIndex = -1;
    
    [SerializeField] private Chapter currentChapter;
    
    [SerializeField] private float starsEarnedThisAct;
    
    [SerializeField] private float starsRequiredToCompleteAct;

    [SerializeField] private ActCanvas actCanvas;

    [SerializeField] private NarrativeSystem overrideIntroCutscene;
    [SerializeField] private NarrativeSystem overrideOutroCutscene;
    
    [SerializeField] private NarrativeSystem defaultCutsceneLayout;
    
    // Actions
    public event Action onChapterOpen; 
    public event Action onChapterClosed;
    public event Action onCutsceneComplete;
    public event Action OnActComplete;
    
    void Awake()
    {
        if (!actCanvas)
        {
            actCanvas = FindObjectOfType<ActCanvas>();
        }
        actCanvas.Initialize(this, chapters);
        LoadActData();

    }

    void Start()
    {
        CheckIfActIsComplete();
    }

    private void OnEnable()
    {
        ChapterInfo.OnChapterStartRequested += LoadChapter;
    }

    private void OnDisable()
    {
        ChapterInfo.OnChapterStartRequested -= LoadChapter;
    }

    private void LoadActData()
    {
        //Load the latest data
        SaveSystem saveSystem = SaveSystem.Instance;
        SaveSystem.UserData userData = saveSystem.GetUserData();

        starsEarnedThisAct = 0.0f;

        // Only compare against chapters in this act
        foreach (SaveSystem.ChapterSaveData saveData in userData.chapterSaveData.Where(saveData => saveData.actNumber == actNumber))
        {
            if (saveData.chapterNumber < chapters.Count)
            {
                float starsEarned = saveData.starsEarned;
                chapters[saveData.chapterNumber].starsEarned = starsEarned;
                starsEarnedThisAct += starsEarned;
            }
            else
            {
                StSDebug.LogError($"Act{actNumber}: Could not load save data for chapter {saveData.chapterNumber} due to invalid index of chapter game objects in the act.");
            }
        }
    }
    
    private void LoadChapter(ChapterStruct chapterToLoad)
    {
        currentChapterIndex = chapters.IndexOf(chapterToLoad);
        if(currentChapterIndex >= 0 && currentChapterIndex < chapters.Count)
        {
            if (SceneLoader.Instance.LoadScene(chapters[currentChapterIndex].sceneInfo))
            {
                onChapterOpen?.Invoke();
                SceneLoader.Instance.onSceneOpened += ChapterLoaded;
            }
        }
        else
        {
            StSDebug.LogError($"Invalid chapter index: {currentChapterIndex}");
        }
    }

    void ChapterLoaded()
    {
        SceneLoader.Instance.onSceneOpened -= ChapterLoaded;
        
        currentChapter = FindObjectOfType<Chapter>();
        if (!currentChapter)
        {
            StSDebug.LogError("Cannot find Chapter");
        }
        else
        {
            currentChapter.onChapterComplete += ChapterComplete;
        }

    }

    void ChapterComplete(float starsEarned)
    {
        currentChapter.onChapterComplete -= ChapterComplete;

        float previousStarsOnChapter = chapters[currentChapterIndex].starsEarned;
        //Update the current chapter as completed
        chapters[currentChapterIndex].starsEarned = starsEarned;
        starsEarnedThisAct += starsEarned - previousStarsOnChapter;
        
        // Save the data!
        SaveSystem saveSystem = SaveSystem.Instance;
        saveSystem.ChapterCompleted(actNumber, currentChapterIndex, starsEarned);
        
        CloseChapter();

        if (CheckIfActIsComplete())
        {
            ProgressToNextAct();
        }
    }
    
    public void ProgressToNextAct()
    {
        SaveSystem.Instance.ActComplete(actNumber);

        if (HasNextAct())
        {
            // Cutscene and next act.
            onCutsceneComplete += GoToNextAct;
            
            if (overrideOutroCutscene && SaveSystem.Instance)
            {
                if (SaveSystem.Instance.HasSeenCutscene(overrideOutroCutscene.GetCutsceneIDForSaveSystem()))
                {
                    onCutsceneComplete?.Invoke();
                    return;
                }
                PlayCutscene(overrideOutroCutscene, NarrativeSO.NarrativeType.Override);
            }
            else
            {
                
                PlayCutscene(null, NarrativeSO.NarrativeType.ActOutro);
            }
        }
    }
    
    private void PlayCutscene(NarrativeSystem cutscene, NarrativeSO.NarrativeType type)
    {
        if (cutscene is null)
        {
            cutscene = Instantiate(defaultCutsceneLayout);
            cutscene.SetParameters(actNumber, type);
        }
        
        // Check if we have already seen this cutscene
        if (SaveSystem.Instance.HasSeenCutscene(cutscene.GetCutsceneIDForSaveSystem()))
        {
            onCutsceneComplete?.Invoke();
            return;
        }
        
        cutscene.gameObject.SetActive(true);
        cutscene.Setup(EndCutscene(cutscene));
    }

    private Action EndCutscene(NarrativeSystem cutscene)
    {
        SaveSystem.Instance.SetCutsceneWatched(cutscene.GetCutsceneIDForSaveSystem());
        cutscene.gameObject.SetActive(true);
        onCutsceneComplete?.Invoke();
        return null;
    }
    

    void GoToNextAct()
    {
        onCutsceneComplete -= GoToNextAct;
        SceneLoader.Instance.LoadScene($"Act {actNumber + 1}");
    }

    void CloseChapter()
    {
        //Close the chapter and clear the current chapter
        SceneLoader.Instance.CloseScene(chapters[currentChapterIndex].sceneInfo);
        currentChapterIndex = -1;
        currentChapter = null;

        onChapterClosed?.Invoke();
    }

    private bool CheckIfActIsComplete()
    {
        foreach (ChapterStruct chapter in chapters)
        {
            if (chapter.starsEarned <= 0.0f)
            {
                return false;
            }
        }

        if (starsEarnedThisAct < starsRequiredToCompleteAct)
        {
            return false;
        }
        
        OnActComplete?.Invoke();
        return true;
    }

    public float GetStarsEarnedInAct()
    {
        return starsEarnedThisAct;
    }

    public int GetActNumber()
    {
        return actNumber;
    }

    public bool HasNextAct()
    {
        return SceneLoader.Instance.CanLoadScene($"Act {actNumber + 1}");
    }
}
