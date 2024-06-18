using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ChapterUI : MonoBehaviour
{
    public Chapter Chapter { get; private set; }

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


    private void Awake()
    {
        _StarDisplay.gameObject.SetActive(false);
        Chapter = FindObjectOfType<Chapter>();

        if (!Chapter)
        {
            StSDebug.LogError($"ChapterUI could not find chapter object.");
        }

        chapterTitle.text = $"Chapter {Chapter.ChapterNumber}";
        
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
        Chapter.onStageChanged += OnStageChanged;
        StagePosition.OnStagePositionClicked += OnStagePositionClicked;
        StagePosition.OnStagePositionChanged += OnStagePositionChanged;
        StageSelection.OnStageSelectionFocusChanged += StagePositionFocusChanged;
        Chapter.onRevealRating += RevealRating;
    }
    
    private void OnDisable()
    {
        if (Chapter is not null)
        {
            Chapter.onRevealRating -= RevealRating;
            Chapter.onStageChanged -= OnStageChanged;
        }
        
        if (onPointerPosition is not null)
        {
            onPointerPosition.performed -= OnPointerPosition;
        }
        
        StagePosition.OnStagePositionClicked -= OnStagePositionClicked;
        StagePosition.OnStagePositionChanged -= OnStagePositionChanged;
        StageSelection.OnStageSelectionFocusChanged -= StagePositionFocusChanged;
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
        bool inStageSelection = Chapter.Instance && Chapter.Instance.IsInCurrentStage(Chapter.ChapterStage.StageSelection);
        bool canClickPosition = !StageSelection.Instance.HasActiveSelection();
        if (inStageSelection && canClickPosition)
        {
            StagePositionFocusChanged(clickedStagePosition);
        }
    }

    private void StagePositionFocusChanged(StagePosition newFocusStagePosition)
    {
        _SelectionCarousels.ShowStageSelection(newFocusStagePosition);
            
        StsCamera stsCamera = StsCamera.Instance;
        if (stsCamera)
        {
            stsCamera.OnStagePositionClicked(newFocusStagePosition);
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
        _graphicRaycaster?.Raycast(pointerData, results);

        return results.Count > 0;
    }
    
    
}
