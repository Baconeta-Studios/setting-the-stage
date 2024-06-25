using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChapterInfo : MonoBehaviour
{
    public ChapterStruct chapter;
    [SerializeField] private TextMeshProUGUI chapterTitle;
    [SerializeField] private StarContainer stars;
    [SerializeField] private Button startButton;
    [SerializeField] private Image bgImageComponent;
    
    public static event Action<ChapterStruct> OnChapterStartRequested;
    
    public void Initialize(ChapterStruct newChapter)
    {
        chapter = newChapter;
        startButton.interactable = false;

        chapterTitle.text = newChapter.sceneInfo.sceneDisplayName;
        bgImageComponent.sprite = newChapter.bgImage;
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
        // This allows the button audio to play TODO could resolve this nicer
        StartCoroutine(DelayedChapterStart());
    }

    private IEnumerator DelayedChapterStart()
    {
        yield return new WaitForSeconds(0.15f);
        OnChapterStartRequested?.Invoke(chapter);
    }
}
