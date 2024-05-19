using System;
using System.Collections;
using System.Collections.Generic;
using CustomEditor;
using TMPro;
using UnityEngine;

public class CarouselItem : MonoBehaviour
{
    public string itemText;
    public TextMeshProUGUI displayText;
    public RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        displayText.text = itemText;
    }
}
