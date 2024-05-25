using System;
using UnityEngine;

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
    private void OnEnable()
    {
        if (!_act)
        {
            _act = FindObjectOfType<Act>();
            if (!_act)
            {
                StSDebug.LogError("Act Canvas could not find Act Object");
            }
        }

        if (_act)
        {
            _act.onChapterOpen += ChapterOpened;
            _act.onChapterClosed += ChapterClosed;
        }
    }

    private void OnDisable()
    {
        if (_act)
        {
            _act.onChapterOpen -= ChapterOpened;
            _act.onChapterClosed -= ChapterClosed;
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
                chapterSelectUI.SetActive(true);
                break;
            case CurrentState.InGame:
                chapterSelectUI.SetActive(false);
                break;
        }
    }
}
