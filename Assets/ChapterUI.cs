using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterUI : MonoBehaviour
{
    private Chapter _chapter;

    [SerializeField]
    private Carousel _SelectionCarousel;
    
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
            case Chapter.ChapterStage.ChooseInstruments:
            case Chapter.ChapterStage.ChooseMusicians:
                break;
            case Chapter.ChapterStage.Performing:
                _SelectionCarousel.gameObject.SetActive(false);
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
        _SelectionCarousel.gameObject.SetActive(true);
    }
    
    
}
