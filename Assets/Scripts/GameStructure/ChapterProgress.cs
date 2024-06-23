using UnityEngine;
using UnityEngine.UI;

public class ChapterProgress : MonoBehaviour
{
    private Button _button;
    private void OnEnable()
    {
        _button = GetComponent<Button>();
    }

    public void ToggleInteractable(bool bEnabled)
    {
        _button.interactable = bEnabled;
    }
}
