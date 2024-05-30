using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class ActCanvas : MonoBehaviour
{
    [Serializable]
    private enum CurrentState
    {
        ChapterSelect,
        InGame,
    }

    [SerializeField] private CurrentState currentState = CurrentState.ChapterSelect;
    private Act _act;

    [SerializeField] private GameObject chapterSelectUI;
    
    [SerializeField] private Transform chapterContainer;
    [SerializeField] private GameObject chapterInfoPrefab;
    [SerializeField] private List<ChapterInfo> chapterInfos = new List<ChapterInfo>();

    void Start()
    {
        StateChanged();
    }
    
    private void OnDisable()
    {
        if (_act)
        {
            _act.onChapterOpen -= ChapterOpened;
            _act.onChapterClosed -= ChapterClosed;
        }
    }

    public void Initialize(Act actParent, List<ChapterStruct> chapters)
    {
        _act = actParent;
        
        if (_act)
        {
            _act.onChapterOpen += ChapterOpened;
            _act.onChapterClosed += ChapterClosed;
        }
        
        foreach (ChapterStruct chapter in chapters)
        {
            ChapterInfo chapterInfo = Instantiate(chapterInfoPrefab, chapterContainer).GetComponent<ChapterInfo>();
            chapterInfo.Initialize(chapter);
            
            chapterInfos.Add(chapterInfo);
        }
    }
    
    void UpdateChapters()
    {
        //Cycle through chapters, and disable locked chapters
        for (int index = 0; index < chapterInfos.Count; index++)
        {
            bool chapterUnlocked = _act.GetStarsEarnedInAct() >= chapterInfos[index].chapter.starsRequiredToUnlock;

            if (chapterUnlocked)
            {
                ChapterInfo chapter = chapterInfos[index];
                chapter.UnlockChapter();
                SaveSystem saveSystem = SaveSystem.Instance;
                if (saveSystem)
                {
                    float stars = saveSystem.GetUserData().GetStarsForChapter(_act.GetActNumber(), index);
                    chapter.StarsChanged(stars);
                }
            }
        }
    }

    public void ChapterOpened()
    {
        currentState = CurrentState.InGame;
        StateChanged();
    }

    public void ChapterClosed()
    {
        currentState = CurrentState.ChapterSelect;
        StateChanged();
    }

    void StateChanged()
    {
        switch (currentState)
        {
            case CurrentState.ChapterSelect:
                UpdateChapters();
                chapterSelectUI.SetActive(true);
                break;
            case CurrentState.InGame:
                chapterSelectUI.SetActive(false);
                break;
        }
    }
}
