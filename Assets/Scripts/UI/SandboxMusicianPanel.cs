using System.Collections;
using System.Collections.Generic;
using Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SandboxMusicianPanel : MusicianInfoPanel
{
    [Header("Panel Parts")] 
    [SerializeField] private GameObject contentArea; // Bio + FunFact container
    [SerializeField] private GameObject trackGrid;   // Grid parent
    [SerializeField] private Button toggleButton;    // Collapse/expand button
    [SerializeField] private TextMeshProUGUI toggleButtonLabel; // optional, for ▲▼

    [Header("Track Button Setup")] 
    [SerializeField] private GameObject trackButtonPrefab; // prefab with TMP + Button
    [SerializeField] private Transform trackButtonParent;  // where track buttons go

    [Header("Animation Settings")] 
    [SerializeField] private float animationDuration = 0.25f;

    private bool _isExpanded = true;
    private LayoutElement _contentLayout;
    private float _expandedHeight = 200f; // tweak in inspector or auto-detect
    private readonly List<Button> _trackButtons = new List<Button>();
    private Instrument _currentlySelectedInstrument;

    protected void Awake()
    {
        // Grab LayoutElement for animating height
        _contentLayout = contentArea.GetComponent<LayoutElement>();
        if (_contentLayout == null)
        {
            _contentLayout = contentArea.AddComponent<LayoutElement>();
        }

        // Optional: auto-detect expanded height
        _expandedHeight = contentArea.GetComponent<RectTransform>().rect.height;

        // Setup toggle button
        toggleButton.onClick.AddListener(TogglePanel);

        // Start with track grid hidden
        trackGrid.SetActive(false);
    }

    private void TogglePanel()
    {
        _isExpanded = !_isExpanded;

        StopAllCoroutines();
        StartCoroutine(AnimatePanel(_isExpanded));

        if (_isExpanded)
        {
            trackGrid.SetActive(false);
            if (toggleButtonLabel) toggleButtonLabel.text = "▲";
        }
        else
        {
            if (_currentlySelectedInstrument == null)
            {
                StSDebug.Log("No instrument selected");
            }
            else
                PopulateTrackButtons(SandboxAudioDataManager.Instance.GetAllTracksForInstrument(_currentlySelectedInstrument));

            trackGrid.SetActive(true);
            if (toggleButtonLabel)
            {
                toggleButtonLabel.text = "▼";
            }
        }
    }

    private IEnumerator AnimatePanel(bool expand)
    {
        RectTransform rect = contentArea.GetComponent<RectTransform>();
        float start = rect.sizeDelta.y;
        float target = expand ? _expandedHeight : 0f;
        float time = 0f;

        // If collapsing, hide children immediately
        if (!expand)
        {
            SetChildrenActive(contentArea.transform, false);
        }

        while (time < 1f)
        {
            time += Time.deltaTime / animationDuration;
            float newHeight = Mathf.Lerp(start, target, time);
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);
            yield return null;
        }

        rect.sizeDelta = new Vector2(rect.sizeDelta.x, target);

        // If expanding, enable children at the end
        if (expand)
            SetChildrenActive(contentArea.transform, true);
        contentArea.SetActive(expand);
    }

    private void SetChildrenActive(Transform parent, bool active)
    {
        foreach (Transform child in parent)
        {
            // Don’t toggle the LayoutElement itself
            child.gameObject.SetActive(active);
        }
    }

    private void PopulateTrackButtons(List<string> trackNames)
    {
        // Clear old buttons
        foreach (Transform child in trackButtonParent)
        {
            Destroy(child.gameObject);
        }

        _trackButtons.Clear();

        foreach (string trackName in trackNames)
        {
            var go = Instantiate(trackButtonPrefab, trackButtonParent);
            var label = go.GetComponentInChildren<TextMeshProUGUI>();
            label.text = trackName;

            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => OnTrackSelected(trackName, btn));

            _trackButtons.Add(btn);
        }

        // Default select first if any
        if (_trackButtons.Count > 0)
            OnTrackSelected(trackNames[0], _trackButtons[0]);
    }

    private void OnTrackSelected(string trackName, Button selectedButton)
    {
        // Reset all buttons to interactable
        foreach (var btn in _trackButtons)
        {
            btn.interactable = true;
        }

        // Disable the one we clicked (visual cue)
        selectedButton.interactable = false;
    }

    protected override void OnStagePositionChanged(StagePosition stagePosition)
    {
        base.OnStagePositionChanged(stagePosition);

        if (_currentlySelectedInstrument != stagePosition.instrumentOccupied && stagePosition.instrumentOccupied != null)
        {
            PopulateTrackButtons(SandboxAudioDataManager.Instance.GetAllTracksForInstrument(stagePosition.instrumentOccupied));
        }

        _currentlySelectedInstrument = stagePosition.instrumentOccupied;
    }
}
