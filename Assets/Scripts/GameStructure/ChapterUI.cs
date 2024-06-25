using System;
using System.Collections.Generic;
using GameStructure;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ChapterUI : MonoBehaviour
{
    public Chapter Chapter { get; private set; }
    public ChapterTrackData ChapterTrackData { get; private set; }

    [SerializeField] private TextMeshProUGUI chapterTitle;
    [SerializeField] private Button trackInfoButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private ChapterPopup chapterPopup;
    [SerializeField] private ChapterProgress _StageProgressButton;
    [SerializeField] private StageSelection _SelectionCarousels;
    [SerializeField] private StarContainer _StarDisplay;
    [SerializeField] private GraphicRaycaster _graphicRaycaster;

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
        ChapterTrackData = Chapter.chapterTrackData;

        chapterTitle.text = ChapterTrackData.chapterTitle;
        
        input = FindObjectOfType<PlayerInput>();
        if (input)
        {
            // Bind to the pointer down event for when to pan
            onPointerPosition = input.actions["PointerPosition"];
            onPointerPosition.performed += OnPointerPosition;
        }
        
        trackInfoButton.onClick.AddListener(() => OpenTrackInfoPopup(CloseChapterPopup));
    }

    private void OnEnable()
    {
        UnhookAllEventBindings();

        Chapter.onStageChanged += OnStageChanged;
        StagePosition.OnStagePositionClicked += OnStagePositionClicked;
        StagePosition.OnStagePositionChanged += OnStagePositionChanged;
        Chapter.onRevealRating += RevealRating;
        StageSelection.OnStageSelectionEnded += ShowChapterTitle;
    }

    private void UnhookAllEventBindings()
    {
        Chapter.onStageChanged -= OnStageChanged;
        StagePosition.OnStagePositionClicked -= OnStagePositionClicked;
        StagePosition.OnStagePositionChanged -= OnStagePositionChanged;
        Chapter.onRevealRating -= RevealRating;
        StageSelection.OnStageSelectionEnded -= ShowChapterTitle;
    }

    private void OnDisable()
    {
        UnhookAllEventBindings();
        
        if (onPointerPosition != null)
        {
            onPointerPosition.performed -= OnPointerPosition;
        }
    }

    private void OnStageChanged(Chapter.ChapterStage chapterStage)
    {
        switch (chapterStage)
        {
            case Chapter.ChapterStage.Intro:
                _StageProgressButton.ToggleInteractable(false);
                OpenTrackInfoPopup(() => Chapter.NextStage());
                HideChapterTitle();
                break;
            case Chapter.ChapterStage.StageSelection:
                ShowChapterTitle();
                CloseChapterPopup();
                settingsButton.interactable = true;
                break;
            case Chapter.ChapterStage.Performing:
                ShowChapterTitle();
                trackInfoButton.gameObject.SetActive(false);
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
            _SelectionCarousels.ShowStageSelection(clickedStagePosition);
            chapterTitle.transform.parent.gameObject.SetActive(false);
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
        _StageProgressButton.ToggleActive(allPositionsOccupied);
        _StageProgressButton.ToggleInteractable(allPositionsOccupied);
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
    
    private void ShowChapterTitle()
    {
        chapterTitle.transform.parent.gameObject.SetActive(true);
    }
    
    private void HideChapterTitle()
    {
        chapterTitle.transform.parent.gameObject.SetActive(false);
    }

    private void OpenTrackInfoPopup(Action onCloseCallback=null)
    {
        HideChapterTitle();
        settingsButton.interactable = false;
        chapterPopup.Init(ChapterTrackData, onCloseCallback);
    }

    private void CloseChapterPopup()
    {
        ShowChapterTitle();
        settingsButton.interactable = true;
        chapterPopup.ClosePopup();
    }
}
