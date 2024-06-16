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

    [SerializeField] private MeshRenderer musicianRenderer;

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
            musicianRenderer.enabled = true;
            musicianRenderer.material.mainTexture = selection.GetSprite().texture;
        }
        else
        {
            musicianRenderer.enabled = false;
            musicianRenderer.material.mainTexture = null;
        }
        
        OnStagePositionChanged?.Invoke(this);
    }
    
    public void InstrumentSelectionChanged(Instrument selection)
    {
        instrumentOccupied = selection;
        
        OnStagePositionChanged?.Invoke(this);
    }

    public bool IsMusicianOccupied()
    {
        return musicianOccupied is not null;
    }
    
    public bool IsInstrumentOccupied()
    {
        return instrumentOccupied is not null;
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
