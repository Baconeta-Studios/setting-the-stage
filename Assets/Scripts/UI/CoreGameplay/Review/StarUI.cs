using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarUI : MonoBehaviour
{

    public GameObject starHalf;
    public GameObject starFull;
    
    public void ShowStar(bool isFull)
    {
        if (isFull)
        {
            starFull.SetActive(true);
        }
        else
        {
            starHalf.SetActive(true);
        }
    }
}
