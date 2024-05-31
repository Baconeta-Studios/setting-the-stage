using System;
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
    
    [SerializeField] private List<GameObject> chapterObjects;
    
    [SerializeField] private int currentChapterIndex = -1;
    
    [SerializeField] private Chapter currentChapter;
    
    [SerializeField] private float starsEarnedThisAct;
    
    // Actions
    public event Action onChapterOpen; 
    public event Action onChapterClosed;
    
    void Start()
    {
        LoadActData();
        UpdateChapters();
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
                chapters[saveData.chapterNumber].starsEarned = saveData.starsEarned;
                starsEarnedThisAct += saveData.starsEarned;
            }
            else
            {
                StSDebug.LogError($"Act{actNumber}: Could not load save data for chapter {saveData.chapterNumber} due to invalid index of chapter game objects in the act.");
            }
        }
    }

    void UpdateChapters()
    {
        if (chapterObjects.Count != chapters.Count)
        {
            StSDebug.LogError($"{chapterObjects.Count} chapter objects found, but only {chapters.Count} are listed. Please check the act {actNumber} object");
            return;
        }
        
        //Cycle through chapters, and disable locked chapters
        for (int index = 0; index < chapterObjects.Count; index++)
        {
            bool chapterUnlocked = starsEarnedThisAct >= chapters[index].starsRequiredToUnlock;

            //TODO Replace with actual chapter objects. Currently these are just button.
            chapterObjects[index].SetActive(chapterUnlocked);
        }
    }
    
    public void LoadChapter(string chapterName)
    {
        currentChapterIndex = chapters.FindIndex(x => x.sceneInfo.sceneDisplayName == chapterName);
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
    }

    void CloseChapter()
    {
        //Close the chapter and clear the current chapter
        SceneLoader.Instance.CloseScene(chapters[currentChapterIndex].sceneInfo);
        currentChapterIndex = -1;
        currentChapter = null;

        //Update the chapters with the current availability.
        UpdateChapters();
        
        onChapterClosed?.Invoke();
    }
}
