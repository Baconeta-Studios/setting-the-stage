using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [SerializeField] private ActCanvas actCanvas;

    //TODO Replace with cutscene objects when they are implemented.
    [SerializeField] private GameObject introCutscene;
    [SerializeField] private GameObject outroCutscene;
    
    // Actions
    public event Action onChapterOpen; 
    public event Action onChapterClosed;
    public event Action onCutsceneComplete;
    public event Action onAllChaptersComplete;
    
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
        CheckIfAllChaptersAreComplete();
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

        if (CheckIfAllChaptersAreComplete())
        {
            ProgressToNextAct();
        }
    }
    
    public void ProgressToNextAct()
    {
        SaveSystem.Instance.ActComplete(actNumber);

        if (HasNextAct())
        {
            // Cut scene and next act.
            StartCoroutine(PlayCutscene(outroCutscene));
            onCutsceneComplete += GoToNextAct;
        }
    }
    
    //TODO Replace GameObject cutscene with a Cutscene object once it is complete.
    IEnumerator PlayCutscene(GameObject cutscene)
    {
        float cutsceneDuration = 0.0f;
        yield return new WaitForSeconds(cutsceneDuration);
        onCutsceneComplete?.Invoke();
        yield return null;
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

    private bool CheckIfAllChaptersAreComplete()
    {
        foreach (ChapterStruct chapter in chapters)
        {
            if (chapter.starsEarned <= 0.0f)
            {
                return false;
            }
        }
        
        onAllChaptersComplete?.Invoke();
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
