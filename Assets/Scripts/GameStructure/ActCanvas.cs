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

    private void Start()
    {
        StateChanged();
    }
    
    private void OnDisable()
    {
        UnhookAllEventBindings();
    }

    private void OnEnable()
    {
        BindActEvents();
    }

    public void Initialize(Act actParent, List<ChapterStruct> chapters)
    {
        _act = actParent;
        
        nextActButton.gameObject.SetActive(false);
        
        BindActEvents();
        
        foreach (ChapterStruct chapter in chapters)
        {
            ChapterInfo chapterInfo = Instantiate(chapterInfoPrefab, chapterContainer).GetComponent<ChapterInfo>();
            chapterInfo.Initialize(chapter);
            
            chapterInfos.Add(chapterInfo);
        }
    }

    private void BindActEvents()
    {
        if (_act)
        {
            UnhookAllEventBindings();

            _act.onChapterOpen += ChapterOpened;
            _act.onChapterClosed += ChapterClosed;
            _act.OnActComplete += ShowNextActButton;
            actTitle.text = _act.GetActName();
        }
    }

    private void UnhookAllEventBindings()
    {
        if (_act == null)
        {
            return;
        }
        _act.onChapterOpen -= ChapterOpened;
        _act.onChapterClosed -= ChapterClosed;
        _act.OnActComplete -= ShowNextActButton;
    }


    private void UpdateChapters()
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

    private void StateChanged()
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

    private void ShowNextActButton()
    {
        if (_act.HasNextAct())
        {
            nextActButton.gameObject.SetActive(true);
            nextActButton.onClick.AddListener(_act.ProgressToNextAct);
        }
    }

    public void SetEnabled(bool enable)
    {
        gameObject.SetActive(enable);
    }
}
