using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MusicianInfoPanel : MonoBehaviour
{
    public TextMeshProUGUI musicianNameText;
    public GameObject panel;

    private void OnEnable()
    {
        StagePosition.OnStagePositionChanged += OnStagePositionChanged;
    }
    
    private void OnDisable()
    {
        StagePosition.OnStagePositionChanged -= OnStagePositionChanged;
    }

    private void OnStagePositionChanged(StagePosition stagePosition)
    {
        UpdatePanel(stagePosition.musicianOccupied);
    }
    
    // Shows or Hides the panel based on musician validity and fills the panel with the information from the musician
    public void UpdatePanel(Musician musician)
    {
        if (musician)
        {
            UpdateDetailsFromMusician(musician);
            panel.SetActive(true);
        }
        else if (panel.activeSelf)
        {
            HidePanel();
        }
    }

    public void UpdateDetailsFromMusician(Musician musician)
    {
        musicianNameText.text = musician.GetName();
    }

    public void HidePanel()
    {
        panel.SetActive(false);
        
        // Clear details
        musicianNameText.text = String.Empty;
        
    }
}
