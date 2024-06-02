using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChapterInfo : MonoBehaviour
{
    public ChapterStruct chapter;
    [SerializeField] private TextMeshProUGUI chapterTitle;
    [SerializeField] private StarContainer stars;
    [SerializeField] private Button startButton;
    public static event Action<ChapterStruct> OnChapterStartRequested;
    
    public void Initialize(ChapterStruct newChapter)
    {
        chapter = newChapter;
        startButton.interactable = false;

        chapterTitle.text = newChapter.sceneInfo.sceneDisplayName;
        // Add other initialization such as chapter name, any buttons, or other text wanted here.
    }
    
    public void UnlockChapter()
    {
        startButton.interactable = true;
    }
    
    public void StarsChanged(float starsEarned)
    {
        stars.ShowStars(starsEarned);
    }

    public void StartChapter()
    {
        OnChapterStartRequested?.Invoke(chapter);
    }
}
