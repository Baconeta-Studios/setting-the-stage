using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class StageSelection : Singleton<StageSelection>
{
    [SerializeField] private Carousel.CarouselType currentShowingCarousel = Carousel.CarouselType.Musician;
    public Carousel instrumentCarousel;
    public Carousel musicianCarousel;
    private StagePosition activeStagePosition = null;

    [Header("Carousel Swapper")] 
    [SerializeField] private Image swapperImage;
    [SerializeField] private Sprite musicianImage;
    [SerializeField] private Sprite instrumentImage;

    private List<StagePosition> _StagePositions = new List<StagePosition>();

    public static event Action OnStageSelectionStarted; 
    public static event Action OnStageSelectionEnded;
    public static event Action<StagePosition> OnStageSelectionFocusChanged; 

    protected override void Awake()
    {
        base.Awake();
        _StagePositions = FindObjectsOfType<StagePosition>().ToList();
        if (_StagePositions.Count == 0)
        {
            StSDebug.LogWarning($"{gameObject.name}: No Stage Positions found.");
        }
        gameObject.SetActive(false);
    }

    private void Start()
    {
        switch (currentShowingCarousel)
        {
            case Carousel.CarouselType.Musician:
                swapperImage.sprite = musicianImage;
                break;
            case Carousel.CarouselType.Instrument:
                swapperImage.sprite = instrumentImage;
                break;
        }
    }

    public void ShowStageSelection(StagePosition newActiveStagePosition)
    {
        activeStagePosition = newActiveStagePosition;
        
        switch (currentShowingCarousel)
        {
            case Carousel.CarouselType.Musician:
                musicianCarousel.OpenCarousel(activeStagePosition);
                break;
            case Carousel.CarouselType.Instrument:
                instrumentCarousel.OpenCarousel(activeStagePosition);
                break;
        }
        gameObject.SetActive(true);
        OnStageSelectionStarted?.Invoke();
    }
    
    public void HideStageSelection()
    {
        activeStagePosition = null;
        
        gameObject.SetActive(false);
        
        switch (currentShowingCarousel)
        {
            case Carousel.CarouselType.Musician:
                musicianCarousel.CloseCarousel();
                break;
            case Carousel.CarouselType.Instrument:
                instrumentCarousel.CloseCarousel();
                break;
        }
        
        OnStageSelectionEnded?.Invoke();
    }

    public void SwapCarousels()
    {
        switch (currentShowingCarousel)
        {
            case Carousel.CarouselType.Musician:
                currentShowingCarousel = Carousel.CarouselType.Instrument;
                swapperImage.sprite = instrumentImage;
                
                musicianCarousel.CloseCarousel();
                instrumentCarousel.OpenCarousel(activeStagePosition);
                break;
            case Carousel.CarouselType.Instrument:
                currentShowingCarousel = Carousel.CarouselType.Musician;
                swapperImage.sprite = musicianImage;
                
                instrumentCarousel.CloseCarousel();
                musicianCarousel.OpenCarousel(activeStagePosition);
                break;
        }
    }

    public List<StagePosition> GetStagePositions()
    {
        return _StagePositions;
    }

    public void MoveCurrentSelectionRight()
    {
        MoveCurrentSelection(1);
    }

    public void MoveCurrentSelectionLeft()
    {
        MoveCurrentSelection(-1);
    }

    private void MoveCurrentSelection(int indexDirection)
    {
        if (_StagePositions.Count <= 0)
        {
            StSDebug.LogError("StageSelection: Stage Positions were not found. How did we even get here.");
            return;
        }
        
        int currentIndex = activeStagePosition.stagePositionNumber;

        // Left subtracts 1, Right adds one.
        currentIndex += indexDirection;
        
        if (currentIndex < 0)
        {
            // Wrap to the right
            currentIndex = _StagePositions.Count - 1;
        }
        else if (currentIndex >= _StagePositions.Count)
        {
            // Wrap to the Left
            currentIndex = 0;
        }
        
        // Simulate a "Click" on the new stage position, this will move the camera, update the carousels, and the active stage position.
        StagePosition newPos = _StagePositions.Find(position => position.stagePositionNumber == currentIndex);
        if (newPos)
        {
            OnStageSelectionFocusChanged?.Invoke(newPos);
        }
        else
        {
            StSDebug.LogError($"StageSelection: Could not find stage position with the index '{currentIndex}'");
        }
    }

    public bool HasActiveSelection()
    {
        return activeStagePosition is not null;
    }
}
