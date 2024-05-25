using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterUI : MonoBehaviour
{
    private Chapter _chapter;

    [SerializeField]
    private TempChapterNavigation _StageProgressButton;
    
    [SerializeField]
    private StageSelection _SelectionCarousels;
    
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
        StagePosition.OnStagePositionChanged += OnStagePositionChanged;
    }
    
    private void OnDisable()
    {
        _chapter.onStageChanged -= OnStageChanged;
        StagePosition.OnStagePositionClicked -= OnStagePositionClicked;
        StagePosition.OnStagePositionChanged -= OnStagePositionChanged;
    }

    private void OnStageChanged(Chapter.ChapterStage chapterStage)
    {
        switch (chapterStage)
        {
            case Chapter.ChapterStage.Intro:
                break;
            case Chapter.ChapterStage.StageSelection:
                _StageProgressButton.gameObject.SetActive(false);
                break;
            case Chapter.ChapterStage.Performing:
                _SelectionCarousels.HideStageSelection();
                _StageProgressButton.ToggleInteractable(false);
                break;
            case Chapter.ChapterStage.Ratings:
                _StageProgressButton.ToggleInteractable(true);
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
            _SelectionCarousels.ShowStageSelection(clickedStagePosition);
        }
    }
    private void OnStagePositionChanged(StagePosition changedStagePosition)
    {
        bool allPositionsOccupied = true;
        
        foreach (StagePosition stagePosition in _SelectionCarousels.GetStagePositions())
        {
            if (!stagePosition.IsInstrumentOccupied() || !stagePosition.IsMusicianOccupied())
            {
                allPositionsOccupied = false;
            }
        }

        //Only show the progress button if all positions are occupied
        _StageProgressButton.gameObject.SetActive(allPositionsOccupied);
    }
    
    
}
