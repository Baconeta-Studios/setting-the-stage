using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterUI : MonoBehaviour
{
    private Chapter _chapter;

    [SerializeField]
    private GameObject _SelectionCarousels;
    
    void Awake()
    {
        _chapter = FindObjectOfType<Chapter>();
        if (!_chapter)
        {
            StSDebug.LogError($"ChapterUI could not find chapter object.");
        }
    }

    private void OnEnable()
    {
        _chapter.onStageChanged += OnStageChanged;
        StagePosition.OnStagePositionClicked += OnStagePositionClicked;
    }
    
    private void OnDisable()
    {
        _chapter.onStageChanged -= OnStageChanged;
        StagePosition.OnStagePositionClicked -= OnStagePositionClicked;
    }

    private void OnStageChanged(Chapter.ChapterStage chapterStage)
    {
        switch (chapterStage)
        {
            case Chapter.ChapterStage.Intro:
                break;
            case Chapter.ChapterStage.StageSelection:
                break;
            case Chapter.ChapterStage.Performing:
                _SelectionCarousels.SetActive(false);
                break;
            case Chapter.ChapterStage.Ratings:
                break;
            default:
                StSDebug.LogWarning("ChapterUI: Unhandled chapter stage when changing stage.");
                break;
        }
    }

    private void OnStagePositionClicked(StagePosition clickedStagePosition)
    {
        if (Chapter.Instance && Chapter.Instance.IsInCurrentStage(Chapter.ChapterStage.StageSelection))
        {
            _SelectionCarousels.SetActive(true);
        }
    }
    
    
}
