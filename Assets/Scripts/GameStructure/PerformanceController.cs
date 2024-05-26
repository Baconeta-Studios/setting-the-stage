using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceController : MonoBehaviour
{
    [SerializeField] private bool isPerforming;

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

    void OnStageChanged(Chapter.ChapterStage stage)
    {
        if (stage == Chapter.ChapterStage.Performing)
        {
            StartPerformance();
        }
    }

    private void StartPerformance()
    {
        if (isPerforming)
        {
            return;
        }
        isPerforming = true;
        
        OnPerformanceStarted?.Invoke();

        float audioDuration = 3.0f; // TODO Replace with CALCULATED audio duration
        
        StartCoroutine(Performance(audioDuration));
    }

    private IEnumerator Performance(float duration)
    {
        //TODO Start audio playback here.
        
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
            talliedProficiency += stagePosition.GetMusicianProficiency();
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
