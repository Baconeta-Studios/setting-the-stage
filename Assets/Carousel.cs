using System;
using System.Collections;
using System.Collections.Generic;
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
        
        string currentMusician = activeStagePosition.musicianOccupied;
        string currentInstrument = activeStagePosition.instrumentOccupied;
        InitializeCarousel(currentMusician, currentInstrument);
    }

    private void InitializeCarousel(string currentMusician, string currentInstrument)
    {
        if (isInitialized)
        {
            return;
        }
        isInitialized = true;
        
        Chapter chapter = Chapter.Instance;
        List<string> items = new List<string>();
        //Get items based on which type of carousel this is.
        switch (carouselType)
        {
            case CarouselType.Musician:
                items = chapter.GetAvailableMusicians();
                break;
            case CarouselType.Instrument:
                items = chapter.GetAvailableInstruments();
                break;
        }

        if (items.Count == 0)
        {
            StSDebug.LogError($"{gameObject.name}: No items to add to carousel when initializing.");
        }

        // Add an empty item
        AddItemToCarousel(string.Empty);
        
        //Get the current selection
        string currentSelection = string.Empty;
        switch (carouselType)
        {
            case CarouselType.Musician:
                if (currentMusician != string.Empty)
                {
                    currentSelection = currentMusician;
                }
                break;
            case CarouselType.Instrument:
                if (currentInstrument != string.Empty)
                {
                    currentSelection = currentInstrument;
                }
                break;
        }

        bool hasSelection = currentSelection != string.Empty;
        
        //Start with the current selected item if we have one.
        selectedItemIndex = hasSelection ? 1 : 0;
        
        if (hasSelection)
        {
            //Add the current selection
            AddItemToCarousel(currentSelection);
        }

        // Add the available items
        //TODO Replace string with a data type
        foreach (string itemName in items)
        {
            AddItemToCarousel(itemName);
        }
        
        // Log items - 1 because there is always 1 empty item.
        StSDebug.Log($"Initialized {gameObject.name} with {contentPanel.childCount - 1} non-empty items");
        
        HighlightItem(_contentItems[selectedItemIndex]);
        
        // Instantly snap to the selected item.
        contentPanel.anchoredPosition = GetItemPosition(_contentItems[selectedItemIndex]);
    }

    private CarouselItem AddItemToCarousel(string itemName)
    {
        CarouselItem newItem = Instantiate(carouselItemPrefab, contentPanel).GetComponent<CarouselItem>();
        newItem.Initialize(this, itemName);
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
    }

    void Update()
    {
        if (!isDragging)
        {
            SnapToItem(_contentItems[selectedItemIndex]);
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
                    Chapter.Instance.ReturnInstrument(_contentItems[selectedItemIndex].itemText);
                    break;
                case CarouselType.Musician:
                    Chapter.Instance.ReturnMusician(_contentItems[selectedItemIndex].itemText);
                    break;
            }
            ClearHighlightOnItem(_contentItems[selectedItemIndex]);
            
            //Update the index
            selectedItemIndex = newSelectedItemIndex;
            
            HighlightItem(_contentItems[selectedItemIndex]);
            
            if (currentStagePosition)
            {
                // TODO Replace Selection text with a struct of the musicians/instruments
                string selectionText = _contentItems[selectedItemIndex].itemText;
                switch (carouselType)
                {
                    case CarouselType.Instrument:
                        Chapter.Instance.ConsumeInstrument(selectionText);
                        currentStagePosition.InstrumentSelectionChanged(selectionText);
                        break;
                    case CarouselType.Musician:
                        Chapter.Instance.ConsumeMusician(selectionText);
                        currentStagePosition.MusicianSelectionChanged(selectionText);
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
