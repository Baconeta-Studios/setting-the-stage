using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StagePosition : MonoBehaviour
{
    public TextMeshPro musicianSelection;
    public TextMeshPro instrumentSelection;
    public static event Action<StagePosition> OnStagePositionClicked;
    public void OnInteract()
    {
        StSDebug.Log($"{gameObject.name}: Interact");
        OnStagePositionClicked?.Invoke(this);
    }

    public void MusicianSelectionChanged(string selection)
    {
        musicianSelection.text = selection;
    }
    
    public void InstrumentSelectionChanged(string selection)
    {
        instrumentSelection.text = selection;
    }
}
