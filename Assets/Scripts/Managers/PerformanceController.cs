using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Audio;
using UnityEngine;

public class PerformanceController : MonoBehaviour
{
    [SerializeField] private bool isPerforming;
    
    [Header("Weightings for this chapter")]
    [SerializeField] [Range(0,100)] private float proficiencyWeightPercent = 50f;
    [SerializeField] [Range(0,100)] private float instrumentWeightPercent = 50f;

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

        if (!Mathf.Approximately(proficiencyWeightPercent + instrumentWeightPercent, 100))
        {
            StSDebug.LogWarning("The weightings in this chapter do not sum to 100 and may cause issues with scoring.");
        }
    }

    private void OnStageChanged(Chapter.ChapterStage stage)
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

        await WaitForClipsToLoad();

        float audioDuration = _audioBuilderSystem.PlayBuiltClips();
        OnPerformanceStarted?.Invoke();

        StartCoroutine(Performance(audioDuration));
    }

    private async Task WaitForClipsToLoad()
    {
        await Task.Delay(100);
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
        float proficiencyStars = GetProficiencyStars();
        float instrumentStars = GetInstrumentStars();

        float weightedAverage = proficiencyStars * (proficiencyWeightPercent / 100) +
                                instrumentStars * (instrumentWeightPercent / 100);
        
        // Double then floor to the nearest integer
        weightedAverage = Mathf.Floor(weightedAverage * 2);

        // Half to get the average proficiency rounded DOWN to the nearest 0.0f or 0.5f
        weightedAverage /= 2;

        StSDebug.Log($"Performance Complete, earned {weightedAverage} Stars");

        return Mathf.Max(0, weightedAverage);
    }

    private static float GetInstrumentStars()
    {
        int talliedInstrumentChoices = 0;
        
        List<StagePosition> stagePositions = StageSelection.Instance.GetStagePositions();
        Chapter chapter = FindObjectOfType<Chapter>();
        if (chapter == null)
        {
            StSDebug.LogError("Chapter object missing from scene.");
            return 0;
        }
        
        foreach (StagePosition stagePosition in stagePositions)
        {
            talliedInstrumentChoices += chapter.chapterTrackData.GetInstrumentScore(stagePosition.instrumentOccupied);
        }

        // Get the average proficiency
        float stars = (float)talliedInstrumentChoices / stagePositions.Count;

        return stars;
    }

    private static float GetProficiencyStars()
    {
        int talliedProficiency = 0;
        
        List<StagePosition> stagePositions = StageSelection.Instance.GetStagePositions();
        
        foreach (StagePosition stagePosition in stagePositions)
        {
            talliedProficiency += stagePosition.GetMusicianProficiencyRaw();
        }

        // Get the average proficiency
        float stars = (float)talliedProficiency / stagePositions.Count;
        return stars;
    }
}
