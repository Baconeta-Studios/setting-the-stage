using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private TextMeshProUGUI actTitle;
    
    [SerializeField] private Transform chapterContainer;
    [SerializeField] private GameObject chapterInfoPrefab;
    [SerializeField] private List<ChapterInfo> chapterInfos = new List<ChapterInfo>();
    [SerializeField] private Button nextActButton;

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
            _act.OnActComplete -= ShowNextActButton;
        }
    }

    public void Initialize(Act actParent, List<ChapterStruct> chapters)
    {
        _act = actParent;
        
        nextActButton.gameObject.SetActive(false);
        
        if (_act)
        {
            _act.onChapterOpen += ChapterOpened;
            _act.onChapterClosed += ChapterClosed;
            _act.OnActComplete += ShowNextActButton;
            actTitle.text = $"Act {_act.GetActNumber()}";
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
            ChapterInfo chapterInfo = chapterInfos[index];
            bool chapterUnlocked = _act.GetStarsEarnedInAct() >= chapterInfo.chapter.starsRequiredToUnlock;

            if (chapterUnlocked)
            {
                chapterInfo.UnlockChapter();
                SaveSystem saveSystem = SaveSystem.Instance;
                if (saveSystem)
                {
                    float stars = saveSystem.GetUserData().GetStarsForChapter(_act.GetActNumber(), index);
                    chapterInfo.StarsChanged(stars);
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

    void ShowNextActButton()
    {
        if (_act.HasNextAct())
        {
            nextActButton.gameObject.SetActive(true);
            nextActButton.onClick.AddListener(_act.ProgressToNextAct);
        }
    }
}
