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
    [Header("Components")]
    private RectTransform _scrollPanel;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentPanel;

    [Header("Control")]
    [SerializeField] private float snapSpeed = 10f;

    private List<CarouselItem> _contentItems = new List<CarouselItem>();

    [Header("Selection")]
    [SerializeField] private bool isDragging = false;
    [SerializeField] private int selectedItemIndex = 0;
    [SerializeField] private Color selectionColour = Color.yellow;
    [SerializeField] private float selectionSizeMultiplier = 1.3f;

    private void OnEnable()
    {
        StagePosition.OnStagePositionClicked += OnStagePositionClicked;
    }

    private void OnDisable()
    {
        StagePosition.OnStagePositionClicked -= OnStagePositionClicked;
    }

    void Start()
    {
        _scrollPanel = scrollRect.GetComponent<RectTransform>();
        
        foreach (RectTransform contentItem in contentPanel)
        {
            _contentItems.Add(contentItem.GetComponent<CarouselItem>());
        }
        
        _contentItems[selectedItemIndex].GetComponent<Image>().color = selectionColour;
        _contentItems[selectedItemIndex].rectTransform.localScale *= selectionSizeMultiplier;
    }

    void Update()
    {
        if (!isDragging)
        {
            SnapToItem(_contentItems[selectedItemIndex].rectTransform);
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

    void SnapToItem(RectTransform target)
    {
        //Get target position in local space of the content panel
        Vector2 targetPosition = (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position)
                                 - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
        
        // Lerp from current to target position.
        contentPanel.anchoredPosition = Vector2.Lerp(contentPanel.anchoredPosition, targetPosition, Time.deltaTime * snapSpeed);
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
            RectTransform previousSelection = _contentItems[selectedItemIndex].rectTransform;
            
            //Update the index
            selectedItemIndex = newSelectedItemIndex;
            RectTransform newSelection = _contentItems[selectedItemIndex].rectTransform;
            
            //Remove the highlight & size from the previous selection
            previousSelection.localScale = Vector3.one;
            previousSelection.GetComponent<Image>().color = Color.white;
            
            //Highlight the current selection and increase the size
            newSelection.localScale *= selectionSizeMultiplier;
            newSelection.GetComponent<Image>().color = selectionColour;

            if (currentStagePosition)
            {
                string selectionText = _contentItems[selectedItemIndex].itemText;
                switch (carouselType)
                {
                    case CarouselType.Instrument:
                        currentStagePosition.InstrumentSelectionChanged(selectionText);
                        break;
                    case CarouselType.Musician:
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

    void OnStagePositionClicked(StagePosition clickedStagePosition)
    {
        if (gameObject.activeSelf)
        {
            currentStagePosition = clickedStagePosition;
        }
        
    }
    
}
