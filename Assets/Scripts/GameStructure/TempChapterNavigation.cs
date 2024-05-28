using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TempChapterNavigation : MonoBehaviour
{
    public TextMeshProUGUI textBox;
    public Chapter chapter;
    private Button button;
    private void OnEnable()
    {
        button = GetComponent<Button>();
        chapter.onStageChanged += StageChanged;
    }
    
    private void OnDisable()
    {
        chapter.onStageChanged -= StageChanged;
    }

    void StageChanged(Chapter.ChapterStage stage)
    {
        textBox.text = stage.ToString();
    }

    public void ToggleInteractable(bool bEnabled)
    {
        button.interactable = bEnabled;
    }
}
