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

    protected override void Awake()
    {
        base.Awake();
        _StagePositions = FindObjectsOfType<StagePosition>().ToList();
        if (_StagePositions.Count == 0)
        {
            StSDebug.LogWarning($"{gameObject.name}: No Stage Positions found.");
        }
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
}
