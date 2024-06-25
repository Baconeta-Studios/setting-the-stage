using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Audio;
using UnityEngine;

public class PerformanceController : MonoBehaviour
{
    [SerializeField] private bool isPerforming;
    private AudioBuilderSystem _audioBuilderSystem;

    public static event Action OnPerformanceStarted; 
    /// <summary>
    /// Action called when the performance ends and carries the number of Starts the performance received.
    /// </summary>
    public static event Action<float> OnPerformanceEnded;

    private void OnEnable()
    {
        Chapter.Instance.onStageChanged += OnStageChanged;
    }

    private void OnDisable()
    {
        Chapter.Instance.onStageChanged -= OnStageChanged;
    }

    private void Awake()
    {
        _audioBuilderSystem = FindObjectOfType<AudioBuilderSystem>();
        if (_audioBuilderSystem is null)
        {
            StSDebug.LogError("Performance Controller can't find an audioBuilderSystem in the scene!");
        }
    }

    void OnStageChanged(Chapter.ChapterStage stage)
    {
        if (stage == Chapter.ChapterStage.Performing)
        {
            StartPerformance();
        }
    }

    private async void StartPerformance()
    {
        if (isPerforming)
        {
            return;
        }
        isPerforming = true;
        
        OnPerformanceStarted?.Invoke();
        
        await WaitForClipsToLoad();
        
        float audioDuration = _audioBuilderSystem.PlayBuiltClips();
        
        StartCoroutine(Performance(audioDuration));
    }

    private async Task WaitForClipsToLoad()
    {
        while (!_audioBuilderSystem.ReadyToPlay)
        {
            await Task.Yield();
        }
    }

    private IEnumerator Performance(float duration)
    {
        yield return new WaitForSeconds(duration); 
        
        EndPerformance();
    }
    
    private void EndPerformance()
    {
        if (!isPerforming)
        {
            return;
        }
        isPerforming = false;
        
        OnPerformanceEnded?.Invoke(CalculateStars());
    }

    private float CalculateStars()
    {
        int talliedProficiency = 0;
        
        List<StagePosition> stagePositions = StageSelection.Instance.GetStagePositions();
        
        foreach (StagePosition stagePosition in stagePositions)
        {
            talliedProficiency += stagePosition.GetMusicianProficiencyRaw();
        }

        // Get the average proficiency
        float stars = (float)talliedProficiency / stagePositions.Count;

        // Double then floor to the nearest integer
        stars = Mathf.Floor(stars * 2);

        // Half to get the average proficiency rounded DOWN to the nearest 0.0f or 0.5f
        stars /= 2;
        
        StSDebug.Log($"Performance Complete, earned {stars} Stars");

        return stars;
    }
}
