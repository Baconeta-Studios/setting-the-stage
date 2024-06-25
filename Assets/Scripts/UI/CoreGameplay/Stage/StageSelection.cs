using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class StageSelection : Singleton<StageSelection>
{
    public Carousel instrumentCarousel;
    public Carousel musicianCarousel;
    public MusicianInfoPanel musicianInfoPanel;
    private StagePosition activeStagePosition = null;

    private List<StagePosition> _StagePositions = new List<StagePosition>();

    private int totalSelectionsCommitted = 0;

    public static event Action<StagePosition> OnStageSelectionStarted; 
    public static event Action OnStageSelectionEnded;
    public static event Action<StagePosition> OnStageSelectionFocusChanged; 

    protected override void Awake()
    {
        base.Awake();
        _StagePositions = FindObjectsOfType<StagePosition>().ToList();
        if (_StagePositions.Count == 0)
        {
            StSDebug.LogWarning($"{gameObject.name}: No Stage Positions found.");
        }
        gameObject.SetActive(false);
    }

    protected void OnEnable()
    {
        StagePosition.OnStagePositionCommitted += IncrementSelectionsCommitted;
    }

    protected void OnDisable()
    {
        StagePosition.OnStagePositionCommitted -= IncrementSelectionsCommitted;
    }

    public void ShowStageSelection(StagePosition newActiveStagePosition)
    {
        if (!gameObject.activeSelf)
        {
            OnStageSelectionStarted?.Invoke(newActiveStagePosition);

            ChangeFocus(newActiveStagePosition);
            
            gameObject.SetActive(true);
        }
    }

    private void ChangeFocus(StagePosition newActiveStagePosition)
    {
        if (activeStagePosition)
        {
            activeStagePosition.OnFocusEnd();
        }
        
        newActiveStagePosition.OnFocusStart();
        activeStagePosition = newActiveStagePosition;

        musicianCarousel.OpenCarousel(activeStagePosition);
        instrumentCarousel.OpenCarousel(activeStagePosition);

        musicianInfoPanel.UpdatePanel(activeStagePosition.musicianOccupied);
        
        OnStageSelectionFocusChanged?.Invoke(activeStagePosition);
    }

    public void HideStageSelection()
    {
        if (gameObject.activeSelf)
        {
            activeStagePosition.OnFocusEnd();
            activeStagePosition = null;

            instrumentCarousel.CloseCarousel();
            musicianCarousel.CloseCarousel();

            musicianInfoPanel.HidePanel();

            OnStageSelectionEnded?.Invoke();
            
            gameObject.SetActive(false);
        }
    }

    private void FocusChanged()
    {
        
    }

    public List<StagePosition> GetStagePositions()
    {
        return _StagePositions;
    }

    public void MoveCurrentSelectionRight()
    {
        MoveCurrentSelection(1);
    }

    public void MoveCurrentSelectionLeft()
    {
        MoveCurrentSelection(-1);
    }

    private void MoveCurrentSelection(int indexDirection)
    {
        if (_StagePositions.Count <= 0)
        {
            StSDebug.LogError("StageSelection: Stage Positions were not found. How did we even get here.");
            return;
        }
        
        int currentIndex = activeStagePosition.stagePositionNumber;

        // Left subtracts 1, Right adds one.
        currentIndex += indexDirection;
        
        if (currentIndex < 0)
        {
            // Wrap to the right
            currentIndex = _StagePositions.Count - 1;
        }
        else if (currentIndex >= _StagePositions.Count)
        {
            // Wrap to the Left
            currentIndex = 0;
        }
        
        // Simulate a "Click" on the new stage position, this will move the camera, update the carousels, and the active stage position.
        StagePosition newPos = _StagePositions.Find(position => position.stagePositionNumber == currentIndex);
        if (newPos)
        {
            ChangeFocus(newPos);
        }
        else
        {
            StSDebug.LogError($"StageSelection: Could not find stage position with the index '{currentIndex}'");
        }
    }

    public bool HasActiveSelection()
    {
        return activeStagePosition != null;
    }

    private void IncrementSelectionsCommitted(StagePosition _)
    {
        totalSelectionsCommitted += 1;
    }

    public int GetTotalSelectionsMade()
    {
        return totalSelectionsCommitted;
    }
}
