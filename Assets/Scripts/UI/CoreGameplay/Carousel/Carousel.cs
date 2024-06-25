using System;
using System.Collections;
using System.Collections.Generic;
using GameStructure;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Carousel : MonoBehaviour
{
    [Serializable]
    public enum CarouselType
    {
        Musician,
        Instrument,
    }

    [SerializeField] protected CarouselType carouselType;
    protected StagePosition currentStagePosition;
    protected bool isInitialized = false;
    
    [Header("Components")]
    [SerializeField] protected RectTransform contentPanel;
    [SerializeField] protected GameObject carouselItemPrefab;

    protected List<CarouselItem> _contentItems = new List<CarouselItem>();

    [Header("Selection")]
    [SerializeField] protected int selectedItemIndex = 0;
    [SerializeField] protected Color selectionColour = Color.yellow;
    [SerializeField] protected float selectionSizeMultiplier = 1.3f;

    public void OpenCarousel(StagePosition activeStagePosition)
    {
        // If opened a new stage position while one was currently open
        if (currentStagePosition && activeStagePosition != currentStagePosition)
        {
            // Close the current carousel
            CloseCarousel();
            // Don't return because we still need to open the new one.
        }
        currentStagePosition = activeStagePosition;
        
        //StSDebug.Log($"Opening Carousel: {gameObject.name}");
        
        Musician currentMusician = activeStagePosition.musicianOccupied;
        Instrument currentInstrument = activeStagePosition.instrumentOccupied;
        InitializeCarousel(currentMusician, currentInstrument);
    }

    private void InitializeCarousel(Musician currentMusician, Instrument currentInstrument)
    {
        if (isInitialized)
        {
            return;
        }
        isInitialized = true;
        
        gameObject.SetActive(true);
        
        Chapter chapter = Chapter.Instance;
        List<StSObject> items = new List<StSObject>();
        //Get items based on which type of carousel this is.
        switch (carouselType)
        {
            case CarouselType.Musician:
                foreach (Musician musician in chapter.GetAvailableMusicians())
                {
                    items.Add((StSObject)musician);
                }
                break;
            case CarouselType.Instrument:
                foreach (Instrument instrument in chapter.GetAvailableInstruments())
                {
                    items.Add((StSObject)instrument);
                }
                break;
        }
        
        //Get the current selection
        StSObject currentSelection = null;
        switch (carouselType)
        {
            case CarouselType.Musician:
                if (currentMusician is not null)
                {
                    currentSelection = currentMusician;
                }
                break;
            case CarouselType.Instrument:
                if (currentInstrument is not null)
                {
                    currentSelection = currentInstrument;
                }
                break;
        }

        bool hasSelection = currentSelection is not null;
        
        //Start with the current selected item if we have one.
        selectedItemIndex = hasSelection ? 1 : 0;
        
        // Add an empty item
        AddItemToCarousel(null);
        
        if (hasSelection)
        {
            //Add the current selection
            AddItemToCarousel(currentSelection);
        }

        // Add the available items
        //TODO Replace string with a data type
        foreach (StSObject item in items)
        {
            AddItemToCarousel(item);
        }
        
        // Log items - 1 because there is always 1 empty item.
        StSDebug.Log($"Opened and Initialized {gameObject.name} with {contentPanel.childCount - 1} non-empty items");
        
        HighlightItem(_contentItems[selectedItemIndex]);
    }

    protected virtual CarouselItem AddItemToCarousel(StSObject stsObject)
    {
        CarouselItem newCarouselItem = Instantiate(carouselItemPrefab, contentPanel).GetComponent<CarouselItem>();
        
        newCarouselItem.Initialize(this, stsObject); // TODO add support for icons
        _contentItems.Add(newCarouselItem);

        return newCarouselItem;
    }

    public void CloseCarousel()
    {
        StSDebug.Log($"Closing Carousel: {gameObject.name}");

        for (int childIndex = contentPanel.childCount - 1; childIndex >= 0; childIndex--)
        {
            Destroy(contentPanel.GetChild(childIndex).gameObject);
        }
        
        _contentItems.Clear();
        currentStagePosition = null;
        isInitialized = false;
        
        gameObject.SetActive(false);
    }

    

    protected virtual void SelectItem(int newSelectedItemIndex)
    {
        if (newSelectedItemIndex != selectedItemIndex)
        {
            switch (carouselType)
            {
                case CarouselType.Instrument:
                    Chapter.Instance.ReturnObject(_contentItems[selectedItemIndex].item);
                    break;
                case CarouselType.Musician:
                    Chapter.Instance.ReturnObject(_contentItems[selectedItemIndex].item);
                    break;
            }
            ClearHighlightOnItem(_contentItems[selectedItemIndex]);
            
            //Update the index
            selectedItemIndex = newSelectedItemIndex;
            
            HighlightItem(_contentItems[selectedItemIndex]);
            
            if (currentStagePosition)
            {
                StSObject selection = _contentItems[selectedItemIndex].item;
                Chapter.Instance.ConsumeObject(selection);
                switch (carouselType)
                {
                    case CarouselType.Instrument:
                        currentStagePosition.InstrumentSelectionChanged((Instrument)selection);
                        break;
                    case CarouselType.Musician:
                        currentStagePosition.MusicianSelectionChanged((Musician)selection);
                        break;
                }
            }
            else
            {
                StSDebug.LogError($"{gameObject.name}: While selecting item, could not find current stage position");
            }
        }
    }

    private void HighlightItem(CarouselItem itemToHighlight)
    {
        //Highlight the current selection and increase the size
        itemToHighlight.transform.localScale = Vector3.one * selectionSizeMultiplier;
        itemToHighlight.GetComponent<Image>().color = selectionColour;
    }

    private void ClearHighlightOnItem(CarouselItem itemToClear)
    {
        //Remove the highlight & size from the previous selection
        itemToClear.transform.localScale = Vector3.one;
        itemToClear.GetComponent<Image>().color = Color.white;
    }

    private void OnStagePositionClicked(StagePosition clickedStagePosition)
    {
        currentStagePosition = clickedStagePosition;
    }
    
}
