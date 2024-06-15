using System.Collections;
using System.Collections.Generic;
using GameStructure;
using UnityEngine;

public class Carousel_Button : Carousel
{
    public void MoveSelectionRight()
    {
        MoveSelection(1);
    }

    public void MoveSelectionLeft()
    {
        MoveSelection(-1);
    }

    private void MoveSelection(int direction)
    {
        int newIndex = selectedItemIndex + direction;

        int maxIndex = _contentItems.Count - 1;
        if (newIndex > maxIndex)
        {
            newIndex = 0;
        }
        else if (newIndex < 0)
        {
            newIndex = maxIndex;
        }
        
        SelectItem(newIndex);
    }

    protected override void SelectItem(int newSelectedItemIndex)
    {
        if (newSelectedItemIndex != selectedItemIndex)
        {
            // Disable the old selection so that only the current selection shows.
            // Do this before base.SelectItem because the selected item index is changed.
            _contentItems[selectedItemIndex].gameObject.SetActive(false);
            _contentItems[newSelectedItemIndex].gameObject.SetActive(true);
            
            base.SelectItem(newSelectedItemIndex);
        }
    }
    
    protected override CarouselItem AddItemToCarousel(StSObject stsObject)
    {
        CarouselItem newCarouselItem = Instantiate(carouselItemPrefab, contentPanel).GetComponent<CarouselItem>();
        
        newCarouselItem.Initialize(this, stsObject); // TODO add support for icons
        _contentItems.Add(newCarouselItem);
        CarouselItem carouselItem = newCarouselItem;
        
        // The Selected Index may be initialized as higher than is currently on the carousel.
        // If this is the case, then this item is NOT active.
        bool shouldBeActive = _contentItems.Count - 1 >= selectedItemIndex && _contentItems[selectedItemIndex] == carouselItem;
        carouselItem.gameObject.SetActive(shouldBeActive);
        
        return carouselItem;
    }
}
