using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backdrop : MonoBehaviour
{
    public static event Action OnBackgroundClicked;
    
    public void BackgroundClicked()
    {
        OnBackgroundClicked?.Invoke();
    }
}
