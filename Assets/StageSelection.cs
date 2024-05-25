using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelection : MonoBehaviour
{
    public Carousel instrumentCarousel;
    public Carousel musicianCarousel;

    public void ShowStageSelection(StagePosition activeStagePosition)
    {
        musicianCarousel.OpenCarousel(activeStagePosition);
        instrumentCarousel.OpenCarousel(activeStagePosition);
        gameObject.SetActive(true);
    }
    
    public void HideStageSelection()
    {
        musicianCarousel.CloseCarousel();
        instrumentCarousel.CloseCarousel();
        gameObject.SetActive(false);
    }
}
