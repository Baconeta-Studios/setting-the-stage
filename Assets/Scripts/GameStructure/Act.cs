using System;
using System.Collections.Generic;
using System.Linq;
using Analytics;
using Audio;
using GameStructure.Narrative;
using Managers;
using UnityEngine;
using Utils;

[Serializable]
public class ChapterStruct
{
    public float starsEarned;
    public float starsRequiredToUnlock;
    public Sprite bgImage;
    public SceneStruct sceneInfo;
}

public class Act : MonoBehaviour
{
    [SerializeField] private int actNumber;
    [SerializeField] private string actName;

    [Header("Chapters")]
    [SerializeField] private List<ChapterStruct> chapters;
    [SerializeField] private int currentChapterIndex = -1;
    [SerializeField] private Chapter currentChapter;
    [SerializeField] private float starsEarnedThisAct;
    [SerializeField] private float starsRequiredToCompleteAct;
    [SerializeField] private ActCanvas actCanvas;
    [SerializeField] private GameObject layoutIntroCutscene;
    [SerializeField] private GameObject layoutOutroCutscene;
    
    // Actions
    public event Action onChapterOpen; 
    public event Action onChapterClosed;
    public event Action onCutsceneComplete;
    public event Action OnActComplete;

    private void Awake()
    {
        if (!actCanvas)
        {
            actCanvas = FindObjectOfType<ActCanvas>();
        }
        actCanvas.Initialize(this, chapters);
        LoadActData();

    }

    private void Start()
    {
        HandleIntroCutScene();
        CheckIfActIsComplete();
    }

    private void HandleIntroCutScene()
    {
        // Play cutscene if it exists and has not played yet - if save system doesn't exist, assume we haven't seen it
        PrepareCutscene(layoutIntroCutscene, NarrativeSo.NarrativeType.ActIntro);
    }

    private void PrepareCutscene(GameObject narrativeLayout, NarrativeSo.NarrativeType type)
    {
        PlayCutscene(narrativeLayout, type);
    }

    private void OnEnable()
    {
        ChapterInfo.OnChapterStartRequested += LoadChapter;
        StagePosition.OnStagePositionCommitted += SendStagePositionCommittedEvent;
    }

    private void OnDisable()
    {
        ChapterInfo.OnChapterStartRequested -= LoadChapter;
        StagePosition.OnStagePositionCommitted -= SendStagePositionCommittedEvent;
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
                SceneLoader.Instance.onSceneOpened += ChapterLoaded;
                onChapterOpen?.Invoke();
                AudioWrapper.Instance.StopAllAudio();
            }
        }
        else
        {
            StSDebug.LogError($"Invalid chapter index: {currentChapterIndex}");
        }
    }

    private void ChapterLoaded()
    {
        SceneLoader.Instance.onSceneOpened -= ChapterLoaded;
        
        currentChapter = FindObjectOfType<Chapter>();
        if (!currentChapter)
        {
            StSDebug.LogError("Cannot find Chapter");
        }
        else
        {
            // Save the data
            SaveSystem saveSystem = SaveSystem.Instance;
            saveSystem.ChapterStarted(actNumber, currentChapterIndex);

            // Send analytics
            SendChapterStartedAnalytics();

            currentChapter.onChapterComplete += ChapterComplete;
        }
    }

    private void SendChapterStartedAnalytics()
    {
        int actID = actNumber;
        int levelID = currentChapterIndex;
        SaveSystem.UserData data = SaveSystem.Instance.GetUserData();
        int totalLevelsStarted = data.GetTotalChaptersCompleted();
        int timesThisLevelStarted = data.GetStartedPlaysForChapter(actID, levelID);
        AnalyticsHandlerBase.Instance.LogEvent("LevelStartedEvent", new Dictionary<string, object>
        {
            { "actIdentifier", actID },
            { "levelIdentifier", levelID },
            { "totalLevelsStarted", totalLevelsStarted},
            { "timesStartedThisLevel",  timesThisLevelStarted}
        });
    }

    private void SendStagePositionCommittedEvent(StagePosition stagePosition)
    {
        int actID = actNumber;
        int levelID = currentChapterIndex;
        int selectionsMade = StageSelection.Instance.GetTotalSelectionsMade();
        var analytics = new Dictionary<string, object>
        {
            { "actIdentifier", actID },
            { "levelIdentifier", levelID },
            { "selectionsMade", selectionsMade },
            { "musicianSelected", stagePosition.musicianOccupied.GetName() },
            { "instrumentSelected", stagePosition.instrumentOccupied.GetName() },
            { "stagePosition", stagePosition.stagePositionNumber }
        };

        AnalyticsHandlerBase.Instance.LogEvent("StagePlacementEvent", analytics);
    }

    private void ChapterComplete(float starsEarned)
    {
        bool abandoned = starsEarned == -1;
        if (abandoned) {
            starsEarned = 0;
        }

        int selectionsMade = StageSelection.Instance.GetTotalSelectionsMade();
        StSDebug.Log($"Chapter completed with '{selectionsMade}' selections made.");

        currentChapter.onChapterComplete -= ChapterComplete;

        float previousStarsOnChapter = chapters[currentChapterIndex].starsEarned;

        // Update the current chapter as completed
        chapters[currentChapterIndex].starsEarned = starsEarned;
        starsEarnedThisAct += starsEarned - previousStarsOnChapter;
        
        // Save the data!
        SaveSystem saveSystem = SaveSystem.Instance;
        saveSystem.ChapterCompleted(actNumber, currentChapterIndex, starsEarned);
        
        // Send analytics
        if (abandoned)
        {
            int chapterState = (int) currentChapter.GetCurrentStage();
            SendChapterAbandonedAnalytics(chapterState, selectionsMade);
        }
        else
        {
            SendChapterCompleteAnalytics(starsEarned, selectionsMade);
        }

        CloseChapter();

        if (CheckIfActIsComplete())
        {
            ProgressToNextAct();
        }
    }

    private void SendChapterAbandonedAnalytics(int chapterState, int selectionsMade)
    {
        int actID = actNumber;
        int levelID = currentChapterIndex;
        var analytics = new Dictionary<string, object>
        {
            { "actIdentifier", actID },
            { "levelIdentifier", levelID },
            { "selectionsMade", selectionsMade },
            { "chapterState", chapterState },
        };

        AnalyticsHandlerBase.Instance.LogEvent("LevelAbandonEvent", analytics);
    }

    private void SendChapterCompleteAnalytics(float score, int selectionsMade)
    {
        int actID = actNumber;
        int levelID = currentChapterIndex;
        SaveSystem.UserData data = SaveSystem.Instance.GetUserData();
        var analytics = new Dictionary<string, object>
        {
            { "actIdentifier", actID },
            { "levelIdentifier", levelID },
            { "selectionsMade", selectionsMade },
            { "score", score },
            { "personalHighscore", data.GetStarsForChapter(actID, levelID)},
            { "wasPerformanceSkipped", false },
            { "timesCompletedThisLevel", data.GetCompletedPlaysForChapter(actID, levelID)}
        };

        AnalyticsHandlerBase.Instance.LogEvent("LevelCompletedEvent", analytics);
    }

    public void ProgressToNextAct()
    {
        SaveSystem.Instance.ActComplete(actNumber);

        if (!HasNextAct()) return;
        
        // Cutscene and next act.
        onCutsceneComplete += GoToNextAct;
            
        PrepareCutscene(layoutOutroCutscene, NarrativeSo.NarrativeType.ActOutro);
    }
    
    private void PlayCutscene(GameObject narrativeLayout, NarrativeSo.NarrativeType type)
    {
        actCanvas.SetEnabled(false);
        
        NarrativeLayout cutsceneLayout = Instantiate(narrativeLayout).GetComponent<NarrativeLayout>();
        NarrativeController cutsceneController = new();
        cutsceneController.SetParameters(actNumber, type);
        cutsceneController.Setup(cutsceneLayout, _ => EndCutscene(cutsceneController));

        // Check if we have already seen this cutscene
        if (SaveSystem.Instance.HasSeenCutscene(cutsceneController.GetCutsceneIDForSaveSystem()))
        {
            // We end the cutscene before it begins
            cutsceneController.EndNarrative();
        }
    }

    private void EndCutscene(NarrativeController cutsceneController)
    {
        SaveSystem.Instance.SetCutsceneWatched(cutsceneController.GetCutsceneIDForSaveSystem());
        actCanvas.SetEnabled(true);
        onCutsceneComplete?.Invoke();
    }
    

    private void GoToNextAct()
    {
        onCutsceneComplete -= GoToNextAct;
        SceneLoader.Instance.LoadScene($"Act {actNumber + 1}");
    }

    private void CloseChapter()
    {
        // Close the chapter and clear the current chapter
        SceneLoader.Instance.CloseScene(chapters[currentChapterIndex].sceneInfo);
        currentChapterIndex = -1;
        currentChapter = null;
        MainMenuAudio.Instance.RestartMenuAudio();
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

    public string GetActName()
    {
        return actName;
    }

    public bool HasNextAct()
    {
        return SceneLoader.Instance.CanLoadScene($"Act {actNumber + 1}");
    }
}
