using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

[Serializable]
public class ChapterStruct
{
    public bool isComplete;
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

        //We've surpassed this act, unlock all chapters.
        if (userData.currentAct > actNumber)
        {
            foreach (ChapterStruct chapter in chapters)
            {
                chapter.isComplete = true;
            }
        }
        // We're current at this act
        else if (userData.currentAct == actNumber)
        {
            // Mark all the chapters that we've already done as completed
            for (int index = 0; index <= userData.currentChapter; index++)
            {
                chapters[index].isComplete = true;
            }
        }
        else // We haven't reached this act yet.
        {
            // No Chapters are unlocked.
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
            //First chapter is always available
            if (index == 0)
            {
                continue;
            }
            
            //TODO Replace with actual chapter objects. Currently these are just button.
            chapterObjects[index].SetActive(chapters[index - 1].isComplete);
            
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

    void ChapterComplete()
    {
        currentChapter.onChapterComplete -= ChapterComplete;
        
        //Update the current chapter as completed
        chapters[currentChapterIndex].isComplete = true;
        
        // Save the data!
        SaveSystem saveSystem = SaveSystem.Instance;
        saveSystem.ChapterCompleted(actNumber, currentChapterIndex);
        
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
