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
    
    public void MoveToNextIncompletePosition()
    {
        MoveCurrentSelection(1, true);
    }

    private void MoveCurrentSelection(int indexDirection, bool skipComplete=false)
    {
        if (_StagePositions.Count <= 0)
        {
            StSDebug.LogError("StageSelection: Stage Positions were not found. How did we even get here.");
            return;
        }
        
        var currentIndex = activeStagePosition.stagePositionNumber;

        var incompleteStagePositions = GetIncompleteStagePositions();
        if (skipComplete && incompleteStagePositions.Count > 0)
        {
            int tryValue = TryWrapToLeft(currentIndex + 1);
            int attempts = 0;
            int maxAttempts = _StagePositions.Count + 1;

            while (attempts < maxAttempts)
            {
                if (IsStagePositionIncomplete(_StagePositions.Find(x => x.stagePositionNumber == tryValue)))
                {
                    currentIndex = tryValue;
                    break;
                }
                tryValue = TryWrapToLeft(tryValue + 1);
                attempts++;

                if (tryValue == currentIndex)
                {
                    StSDebug.LogError("MoveCurrentSelection: Wrapped all the way around, no incomplete stage found.");
                    return;
                }
            }
        }
        else
        {
            // Left subtracts 1, Right adds one.
            currentIndex += indexDirection;
        
            if (currentIndex < 0)
            {
                // Wrap to the far right
                currentIndex = _StagePositions.Count - 1;
            }
            else
            {
                // Wrap to the far left
                currentIndex = TryWrapToLeft(currentIndex);
            }
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

    private int TryWrapToLeft(int value)
    {
        if (value >= _StagePositions.Count)
        {
            value = 0;
        }

        return value;
    }

    private List<StagePosition> GetIncompleteStagePositions()
    {
        return _StagePositions.Where(IsStagePositionIncomplete).ToList();
    }

    private bool IsStagePositionIncomplete(StagePosition position)
    {
        return position.musicianOccupied == null || position.instrumentOccupied == null;
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
