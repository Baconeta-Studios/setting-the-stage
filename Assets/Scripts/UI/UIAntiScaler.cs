using UnityEngine;
using UnityEngine.UI;

public class UIAntiScaler : MonoBehaviour
{
    private void Start()
    {
        var scaler = FindAnyObjectByType<CanvasScaler>();
        this.GetComponent<Image>().pixelsPerUnitMultiplier = scaler.transform.localScale.x;
    }
}
