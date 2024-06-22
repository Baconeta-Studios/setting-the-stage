using System;
using TMPro;
using UnityEngine;

public class StagePosition : MonoBehaviour
{
    public static event Action<StagePosition> OnStagePositionClicked;
    public static event Action<StagePosition> OnStagePositionChanged;
    
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
        }

        OnStagePositionChanged?.Invoke(this);
    }
    
    public void InstrumentSelectionChanged(Instrument selection)
    {
        instrumentOccupied = selection;
        
        if (instrumentOccupied)
        {
            instrumentOccupied.transform.SetParent(musicianOrigin);
            instrumentOccupied.gameObject.SetActive(true);
            if (IsMusicianOccupied())
            {
                musicianOccupied.EquipInstrument(instrumentOccupied);
                // TODO Now set animation controller to equipping this instrument
            }
        }
        
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
        spotlight.enabled = true;
        spotlightMesh.SetActive(true);
    }

    public void OnFocusEnd()
    {
        spotlight.enabled = false;
        spotlightMesh.SetActive(false);

    }
}
