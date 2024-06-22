using UnityEngine;
using UnityEngine.UI;

public class Carousel_Scroll : Carousel
{
    [SerializeField] private bool isDragging = false;
    [Header("Control")]
    [SerializeField] private float snapSpeed = 10f;

    [SerializeField] private ScrollRect scrollRect;
    private Transform scrollPanel;
    
    private void Start()
    {
        scrollPanel = scrollRect.transform;
    }
    private void Update()
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
            
            float distance = Vector2.Distance(scrollPanel.position, itemPos);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestItemIndex = index;
            }
        }

        SelectItem(closestItemIndex);
    }
}
