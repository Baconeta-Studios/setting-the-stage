using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarouselItem : MonoBehaviour
{
    public string itemText;
    public RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
}
