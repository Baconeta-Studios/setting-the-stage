using System;
using GameStructure;
using TMPro;
using UnityEngine;

public class CarouselItem : MonoBehaviour
{
    // This is used to denote "Clear Selection"
    private bool _isEmpty = false;
    
    public StSObject item;
    public TextMeshProUGUI displayText;
    public RectTransform rectTransform;

    public void Initialize(Carousel parent, StSObject newItem)
    {
        rectTransform = GetComponent<RectTransform>();
        item = newItem;
        
        string itemName = "Empty";
        if (newItem is null)
        {
            _isEmpty = true;
        }
        else
        {
            itemName = newItem.GetName();
        }
        gameObject.name = "CarouselItem_" + itemName;
        
        displayText.text = itemName;
    }

    // Checks if this carousel item is an empty item.
    // This is used to denote "Clear Selection"
    public bool IsEmpty()
    {
        return _isEmpty;
    }
}
