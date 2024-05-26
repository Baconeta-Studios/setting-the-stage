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

    [SerializeField] private CarouselType carouselType;
    private StagePosition currentStagePosition;
    private bool isInitialized = false;
    
    [Header("Components")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentPanel;
    private RectTransform _scrollPanel;
    [SerializeField] private GameObject carouselItemPrefab;

    [Header("Control")]
    [SerializeField] private float snapSpeed = 10f;

    private List<CarouselItem> _contentItems = new List<CarouselItem>();

    [Header("Selection")]
    [SerializeField] private bool isDragging = false;
    [SerializeField] private int selectedItemIndex = 0;
    [SerializeField] private Color selectionColour = Color.yellow;
    [SerializeField] private float selectionSizeMultiplier = 1.3f;

    void Start()
    {
        _scrollPanel = scrollRect.GetComponent<RectTransform>();
    }

    public void OpenCarousel(StagePosition activeStagePosition)
    {
        currentStagePosition = activeStagePosition;
        
        StSDebug.Log($"Opening Carousel: {gameObject.name}");
        
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

        if (items.Count == 0)
        {
            StSDebug.LogError($"{gameObject.name}: No items to add to carousel when initializing.");
        }

        // Add an empty item
        AddItemToCarousel(null);
        
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
        StSDebug.Log($"Initialized {gameObject.name} with {contentPanel.childCount - 1} non-empty items");
        
        HighlightItem(_contentItems[selectedItemIndex]);
    }

    private CarouselItem AddItemToCarousel(StSObject item)
    {
        CarouselItem newItem = Instantiate(carouselItemPrefab, contentPanel).GetComponent<CarouselItem>();
        newItem.Initialize(this, item); // TODO add support for icons
        _contentItems.Add(newItem);

        return newItem;
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

    void Update()
    {
        if (!isDragging)
        {
            if (selectedItemIndex < _contentItems.Count)
            {
                SnapToItem(_contentItems[selectedItemIndex]);
            }
        }
        else
        {
            FindClosestItem();
        }
    }

    public void OnDragStart()
    {
        isDragging = true;
    }

    public void OnDragEnd()
    {
        isDragging = false;
        FindClosestItem();
    }

    void SnapToItem(CarouselItem target)
    {
        // Lerp from current to target position.
        contentPanel.anchoredPosition = Vector2.Lerp(contentPanel.anchoredPosition, GetItemPosition(target), Time.deltaTime * snapSpeed);
    }

    Vector2 GetItemPosition(CarouselItem target)
    {
        //Get target position in local space of the content panel
        Vector2 targetPosition = (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position)
                                 - (Vector2)scrollRect.transform.InverseTransformPoint(target.rectTransform.position);
        
        return targetPosition;
    }

    void FindClosestItem()
    {
        float closestDistance = float.MaxValue;
        int closestItemIndex = selectedItemIndex;
        for (int index = 0; index < _contentItems.Count; index++)
        {
            Vector2 itemPos = _contentItems[index].rectTransform.position;
            
            float distance = Vector2.Distance(_scrollPanel.position, itemPos);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestItemIndex = index;
            }
        }

        SelectItem(closestItemIndex);
    }

    void SelectItem(int newSelectedItemIndex)
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

    void HighlightItem(CarouselItem itemToHighlight)
    {
        //Highlight the current selection and increase the size
        itemToHighlight.transform.localScale = Vector3.one * selectionSizeMultiplier;
        itemToHighlight.GetComponent<Image>().color = selectionColour;
    }

    void ClearHighlightOnItem(CarouselItem itemToClear)
    {
        //Remove the highlight & size from the previous selection
        itemToClear.transform.localScale = Vector3.one;
        itemToClear.GetComponent<Image>().color = Color.white;
    }

    void OnStagePositionClicked(StagePosition clickedStagePosition)
    {

        currentStagePosition = clickedStagePosition;
        
    }
    
}
