using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ChapterUI : MonoBehaviour
{
    private Chapter _chapter;

    [SerializeField] private TextMeshProUGUI chapterTitle;
    
    [SerializeField]
    private TempChapterNavigation _StageProgressButton;
    
    [SerializeField]
    private StageSelection _SelectionCarousels;
    
    [SerializeField]
    private StarContainer _StarDisplay;
    
    [SerializeField]
    private GraphicRaycaster _graphicRaycaster;

    private PlayerInput input;
    private InputAction onPointerPosition;
    private Vector2 pointerPosition;


    void Awake()
    {
        _StarDisplay.gameObject.SetActive(false);
        _chapter = FindObjectOfType<Chapter>();

        if (!_chapter)
        {
            StSDebug.LogError($"ChapterUI could not find chapter object.");
        }

        chapterTitle.text = $"Chapter {_chapter.ChapterNumber}";
        
        input = FindObjectOfType<PlayerInput>();
        if (input)
        {
            // Bind to the pointer down event for when to pan
            onPointerPosition = input.actions["PointerPosition"];
            onPointerPosition.performed += OnPointerPosition;
        }
    }

    private void OnEnable()
    {
        _chapter.onStageChanged += OnStageChanged;
        StagePosition.OnStagePositionClicked += OnStagePositionClicked;
        StagePosition.OnStagePositionChanged += OnStagePositionChanged;
        _chapter.onRevealRating += RevealRating;
    }
    
    private void OnDisable()
    {
        _chapter.onStageChanged -= OnStageChanged;
        StagePosition.OnStagePositionClicked -= OnStagePositionClicked;
        StagePosition.OnStagePositionChanged -= OnStagePositionChanged;
        _chapter.onRevealRating -= RevealRating;
        
        onPointerPosition.performed -= OnPointerPosition;
    }

    private void OnStageChanged(Chapter.ChapterStage chapterStage)
    {
        switch (chapterStage)
        {
            case Chapter.ChapterStage.Intro:
                break;
            case Chapter.ChapterStage.StageSelection:
                _StageProgressButton.gameObject.SetActive(false);
                break;
            case Chapter.ChapterStage.Performing:
                _SelectionCarousels.HideStageSelection();
                _StageProgressButton.ToggleInteractable(false);
                break;
            case Chapter.ChapterStage.Ratings:
                _StageProgressButton.ToggleInteractable(true);
                break;
            default:
                StSDebug.LogWarning("ChapterUI: Unhandled chapter stage when changing stage.");
                break;
        }
    }

    private void OnStagePositionClicked(StagePosition clickedStagePosition)
    {
        if (Chapter.Instance && Chapter.Instance.IsInCurrentStage(Chapter.ChapterStage.StageSelection))
        {
            _SelectionCarousels.ShowStageSelection(clickedStagePosition);
        }
    }
    private void OnStagePositionChanged(StagePosition changedStagePosition)
    {
        bool allPositionsOccupied = true;
        
        foreach (StagePosition stagePosition in _SelectionCarousels.GetStagePositions())
        {
            if (!stagePosition.IsInstrumentOccupied() || !stagePosition.IsMusicianOccupied())
            {
                allPositionsOccupied = false;
            }
        }

        //Only show the progress button if all positions are occupied
        _StageProgressButton.gameObject.SetActive(allPositionsOccupied);
    }

    private void RevealRating(float starsEarned)
    {
        _StarDisplay.gameObject.SetActive(true);
        _StarDisplay.ShowStars(starsEarned);
    }

    private void OnPointerPosition(InputAction.CallbackContext context)
    {
        pointerPosition = context.ReadValue<Vector2>();
    }
    
    public bool IsPointerOverUi()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = pointerPosition
        };
        
        List<RaycastResult> results = new List<RaycastResult>();
        
        //Ray cast UI elements only.
        _graphicRaycaster.Raycast(pointerData, results);

        return results.Count > 0;
    }
    
    
}
