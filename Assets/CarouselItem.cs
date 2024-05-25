using System;
using TMPro;
using UnityEngine;

public class CarouselItem : MonoBehaviour
{
    private Carousel carousel = null;
    // This is used to denote "Clear Selection"
    private bool _isEmpty = false;
    
    public string itemText;
    public TextMeshProUGUI displayText;
    public RectTransform rectTransform;

    public void Initialize(Carousel parent)
    {
        Initialize(parent, string.Empty);
    }
    
    public void Initialize(Carousel parent, string itemName)
    {
        rectTransform = GetComponent<RectTransform>();
        if (itemName == string.Empty)
        {
            _isEmpty = true;
        }
        itemText = itemName;
        gameObject.name = "CarouselItem_" + itemText;
        
        displayText.text = itemText;
    }

    // Checks if this carousel item is an empty item.
    // This is used to denote "Clear Selection"
    public bool IsEmpty()
    {
        return _isEmpty;
    }
}
