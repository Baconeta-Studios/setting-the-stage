using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TempChapterNavigation : MonoBehaviour
{
    public TextMeshProUGUI textBox;
    public Chapter chapter;

    private void OnEnable()
    {
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
}
