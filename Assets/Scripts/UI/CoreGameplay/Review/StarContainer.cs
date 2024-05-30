using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarContainer : MonoBehaviour
{
    public void ShowStars(float starsEarned)
    {
        StarUI[] stars = GetComponentsInChildren<StarUI>();
        
        for (int i = 0; i < stars.Length; i++)
        {
            // Full Star
            if (starsEarned >= i + 1)
            {
                stars[i].ShowStar(true);
            }
            // Half Star | If its greater than the index, then its a 0.5 value.
            else if(starsEarned > i)
            {
                stars[i].ShowStar(false);
            }
            // Else leave the star inactive.
        }
    }
}
