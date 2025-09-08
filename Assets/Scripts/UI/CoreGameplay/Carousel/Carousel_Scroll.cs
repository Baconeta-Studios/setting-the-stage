using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem; // New input system

public class Carousel_Scroll : Carousel
{
    [SerializeField] private bool isDragging = false;
    [Header("Control")]
    [SerializeField] private float snapSpeed = 10f;

    [SerializeField] private ScrollRect scrollRect;
    private Transform scrollPanel;

    // Input actions
    private InputAction clickAction;
    private InputAction positionAction;

    private void Awake()
    {
        // Setup actions manually (alternative: generate C# class from Input Actions asset)
        clickAction = new InputAction("Click", binding: "<Pointer>/press");
        positionAction = new InputAction("Position", binding: "<Pointer>/position");

        clickAction.Enable();
        positionAction.Enable();
    }

    private void Start()
    {
        scrollPanel = scrollRect.transform;
    }

    private void Update()
    {
        if (!isDragging)
        {
            HandleCarouselItemClick();

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

    private void HandleCarouselItemClick()
    {
        if (clickAction.WasPressedThisFrame())
        {
            Vector2 inputPos = positionAction.ReadValue<Vector2>();

            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = inputPos
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (var result in results)
            {
                var item = result.gameObject.GetComponent<CarouselItem>();
                if (item != null)
                {
                    SelectItem(item.transform.parent.GetSiblingIndex());
                    break;
                }
            }
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
        contentPanel.anchoredPosition = Vector2.Lerp(
            contentPanel.anchoredPosition,
            GetItemPosition(target),
            Time.deltaTime * snapSpeed
        );
    }

    Vector2 GetItemPosition(CarouselItem target)
    {
        Vector2 targetPosition = (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position)
                                 - (Vector2)scrollRect.transform.InverseTransformPoint(target.rectTransform.position);

        return targetPosition;
    }

    private void FindClosestItem()
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
