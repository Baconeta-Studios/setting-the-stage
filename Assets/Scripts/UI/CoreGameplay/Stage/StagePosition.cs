using System;
using UnityEngine;

public class StagePosition : MonoBehaviour
{
    public static event Action<StagePosition> OnStagePositionClicked;
    public static event Action<StagePosition> OnStagePositionChanged;
    public static event Action<StagePosition> OnStagePositionCommitted;
    
    [SerializeField] private Transform viewTarget;
    
    [Header("Stage Parameters")]
    public int stagePositionNumber;
    
    [Header("Musician")]
    public Musician musicianOccupied = null;

    [SerializeField] private Transform musicianOrigin;
    [SerializeField] private GameObject musicianGameObject;

    [Header("Instrument")]
    public Instrument instrumentOccupied = null;

    [Header("Lighting")] 
    [SerializeField] private Light spotlight;
    [SerializeField] private GameObject spotlightMesh;
    [SerializeField] private GameObject floorMarkerSystem;

    private bool _hasUncommittedChanges = false;

    private void OnEnable()
    {
        StageSelection.OnStageSelectionStarted += OnStageSelectionStart;
        StageSelection.OnStageSelectionEnded += OnStageSelectionEnd;
    }
    
    private void OnDisable()
    {
        StageSelection.OnStageSelectionStarted -= OnStageSelectionStart;
        StageSelection.OnStageSelectionEnded -= OnStageSelectionEnd;
    }

    public void OnInteract()
    {
        OnStagePositionClicked?.Invoke(this);
    }

    public void MusicianSelectionChanged(Musician selection)
    {
        musicianOccupied = selection;

        if (musicianOccupied)
        {
            Transform musicianTransform = musicianOccupied.transform;
            musicianTransform.SetParent(musicianOrigin);
            musicianTransform.localPosition = Vector3.zero;
            
            musicianOccupied.gameObject.SetActive(true);
            if (instrumentOccupied)
            {
                musicianOccupied.EquipInstrument(instrumentOccupied);
                musicianOccupied.SetAnimationBool(instrumentOccupied.AnimationHoldName, true);
            }
        }

        _hasUncommittedChanges = true;
        OnStagePositionChanged?.Invoke(this);
    }
    
    public void InstrumentSelectionChanged(Instrument selection)
    {
        Instrument lastInstrument = instrumentOccupied;
        instrumentOccupied = selection;
        
        if (instrumentOccupied)
        {
            if (lastInstrument && musicianOccupied)
            {
                musicianOccupied.SetAnimationBool(lastInstrument?.AnimationHoldName, false);
            }
            instrumentOccupied.transform.SetParent(musicianOrigin);
            instrumentOccupied.gameObject.SetActive(true);
            if (IsMusicianOccupied())
            {
                musicianOccupied.EquipInstrument(instrumentOccupied);
                musicianOccupied.SetAnimationBool(instrumentOccupied.AnimationHoldName, true);
            }
        }
        else
        {
            if (musicianOccupied)
            {
                musicianOccupied.UnequipInstrument();
                if (lastInstrument)
                {
                    musicianOccupied.SetAnimationBool(lastInstrument?.AnimationHoldName, false);
                }
            }
        }

        _hasUncommittedChanges = true;
        OnStagePositionChanged?.Invoke(this);
    }

    public bool IsMusicianOccupied()
    {
        return musicianOccupied != null;
    }
    
    public bool IsInstrumentOccupied()
    {
        return instrumentOccupied != null;
    }

    public InstrumentProficiency GetMusicianProficiency()
    {
        return musicianOccupied.GetInstrumentProficiency(instrumentOccupied);
    }
    
    public int GetMusicianProficiencyRaw()
    {
        return (int)musicianOccupied.GetInstrumentProficiency(instrumentOccupied);
    }
    
    public Transform GetViewTarget()
    {
        return viewTarget; 
    }

    public void OnFocusStart()
    {
        BrightenLights();
    }

    public void OnFocusEnd()
    {
        CommitSelection();
        DimLights();
    }

    private void OnStageSelectionStart(StagePosition unusedPosition)
    {
        HideFloorMarker();
    }

    private void OnStageSelectionEnd()
    {
        DimLights();
        if (!musicianOccupied)
        {
            ShowFloorMarker();
        }
    }
    
    private void BrightenLights()
    {
        spotlight.enabled = true;
        spotlightMesh.SetActive(true);
    }

    private void DimLights()
    {
        spotlight.enabled = false;
        spotlightMesh.SetActive(false);
    }

    private void ShowFloorMarker()
    {
        if (!musicianOccupied)
        {
            floorMarkerSystem.SetActive(true);
        }
    }

    private void HideFloorMarker()
    {
        floorMarkerSystem.SetActive(false);
    }

    private void CommitSelection()
    {
        // We only want this to occur once per set of changes
        if (_hasUncommittedChanges && musicianOccupied && instrumentOccupied)
        {
            OnStagePositionCommitted?.Invoke(this);
            _hasUncommittedChanges = false;
        }
    }
}
