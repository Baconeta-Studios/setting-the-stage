using GameStructure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarouselItem : MonoBehaviour
{
    // This is used to denote "Clear Selection"
    private bool _isEmpty = false;
    
    public StSObject item;
    public TextMeshProUGUI displayText;
    public RectTransform rectTransform;
    public Image spriteImage;

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
        
        if (displayText)
        {
            displayText.text = itemName;
        }
        
        // Handle instrument sprites as special case
        if (newItem == null && spriteImage != null)
        {
            spriteImage.enabled = false;
            return;
        }
        Sprite sprite = (newItem as Instrument)?.InstrumentSprite;
        if (sprite != null && spriteImage != null)
        {
            spriteImage.sprite = sprite;
        }
    }

    // Checks if this carousel item is an empty item.
    // This is used to denote "Clear Selection"
    public bool IsEmpty()
    {
        return _isEmpty;
    }
}
