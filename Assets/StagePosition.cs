using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StagePosition : MonoBehaviour
{
    public static event Action<StagePosition> OnStagePositionClicked;
    
    [Header("Musician")]
    public string musicianOccupied = string.Empty;
    public TextMeshPro musicianSelection;
    
    [Header("Instrument")]
    public string instrumentOccupied = string.Empty;
    public TextMeshPro instrumentSelection;

    public void OnInteract()
    {
        StSDebug.Log($"{gameObject.name}: Interact");
        OnStagePositionClicked?.Invoke(this);
    }

    public void MusicianSelectionChanged(string selection)
    {
        musicianSelection.text = musicianOccupied = selection;
    }
    
    public void InstrumentSelectionChanged(string selection)
    {
        instrumentSelection.text = instrumentOccupied = selection;
    }

    public bool IsMusicianOccupied()
    {
        return musicianOccupied == string.Empty;
    }
    public bool IsInstrumentOccupied()
    {
        return instrumentOccupied == string.Empty;
    }
}
