using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Carousel : MonoBehaviour
{

    [SerializeField] private RectTransform scrollPanel;
    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private RectTransform contentPanel;

    [SerializeField] private float snapSpeed = 10f;

    [SerializeField] private List<RectTransform> contentItems;

    [SerializeField] private int selectedItemIndex = 0;

    [SerializeField] private bool isDragging = false;
    
    void Start()
    {
        foreach (RectTransform contentItem in contentPanel)
        {
            contentItems.Add(contentItem);
        }
        
        contentItems[selectedItemIndex].GetComponent<Image>().color = Color.yellow;

    }

    void Update()
    {
        if (!isDragging)
        {
            SnapToItem(contentItems[selectedItemIndex]);
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
        for (int index = 0; index < contentItems.Count; index++)
        {
            Vector2 itemPos = contentItems[index].position;
            
            float distance = Vector2.Distance(scrollPanel.position, itemPos);

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
            RectTransform previousSelection = contentItems[selectedItemIndex];
            
            //Update the index
            selectedItemIndex = newSelectedItemIndex;
            RectTransform newSelection = contentItems[selectedItemIndex];
            
            //Remove the highlight from the previous selection
            previousSelection.GetComponent<Image>().color = Color.white;
            
            //Highlight the current selection
            newSelection.GetComponent<Image>().color = Color.yellow;
        }
    }
    
}
