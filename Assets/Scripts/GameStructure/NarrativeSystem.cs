using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace GameStructure
{
    public class NarrativeSystem : MonoBehaviour
    {
        [SerializeField] private Image[] panels;
        [SerializeField] private Image fullScreenPanel;

        [SerializeField] private Button backButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button skipAllButton;
        [SerializeField] private Button[] allButtonsToPauseAutoScroll;

        [SerializeField] private float timeToMoveToNextPanel = 10f; // Time in seconds

        private float _timeUntilTrigger;

        private int _actNumber;
        private NarrativeSO.NarrativeType _cutsceneType;
        private NarrativeSO _thisNarrative;
        private NarrativeDataManager _dataManagerRef;
        private readonly List<int> _panelsPerPage = new();
        private int _pageOnScreen = -1;

        private Action<NarrativeSystem> _actionOnEnd;

        public void Update()
        {
            _timeUntilTrigger -= Time.deltaTime;

            if (_timeUntilTrigger <= 0f)
            {
                MoveToNextPage(); // A page consists of panels.Length # of panels
            }
        }

        private void ShowFirstPanel()
        {
            backButton.interactable = false;
            ShowPage(0);
        }

        private void ShowPage(int pageOfPanels)
        {
            int totalPanelsBeforeThisPage = _panelsPerPage.Take(pageOfPanels).Sum();

            if (_panelsPerPage[pageOfPanels] == 1)
            {
                // Show single panel only
                fullScreenPanel.sprite = _thisNarrative.allPanels[totalPanelsBeforeThisPage].panelImage;
                fullScreenPanel.gameObject.SetActive(true);
                foreach (Image p in panels)
                {
                    p.gameObject.SetActive(false);
                }
            }
            else // We will populate each panel on the page
            {
                fullScreenPanel.gameObject.SetActive(false);
                for (var i = 0; i < panels.Length; i++)
                {
                    Sprite thisPanelSprite = _thisNarrative.allPanels[totalPanelsBeforeThisPage + i].panelImage;
                    panels[i].sprite = thisPanelSprite;
                    panels[i].gameObject.SetActive(true);
                }
            }

            _pageOnScreen = pageOfPanels;
        }

        private void ResetTimer()
        {
            _timeUntilTrigger = timeToMoveToNextPanel;
        }

        public void MoveToNextPage()
        {
            ResetTimer();

            // move to next page of panels or end narrative
            if (_pageOnScreen + 1 >= _panelsPerPage.Count)
            {
                EndNarrative();
            }
            else
            {
                ShowPage(_pageOnScreen + 1);
            }

            backButton.interactable = true;
        }

        public void EndNarrative()
        {
            _actionOnEnd?.Invoke(this); // could consider event triggers also but this seems ok for now
        }

        public void MoveToPreviousPage()
        {
            ResetTimer();

            // move to prev page of panels if it exists
            if (_pageOnScreen > 0)
            {
                ShowPage(_pageOnScreen - 1);
                if (_pageOnScreen == 0)
                {
                    backButton.interactable = false;
                }
            }
        }

        private void OnUserInteraction()
        {
            ResetTimer();
        }

        private void OnEnable()
        {
            foreach (Button button in allButtonsToPauseAutoScroll)
            {
                button.onClick.AddListener(OnUserInteraction);
            }
        }

        private void OnDisable()
        {
            foreach (Button button in allButtonsToPauseAutoScroll)
            {
                button.onClick.RemoveListener(OnUserInteraction);
            }
        }

        // This system should calculate the number of screens needed to show all panels 
        public void Setup(Action<NarrativeSystem> invokeOnEnd)
        {
            _actionOnEnd = invokeOnEnd;

            // Populate data for the system from the narrative manager
            _dataManagerRef = NarrativeDataManager.Instance;
            _thisNarrative = _dataManagerRef.GetNarrativeData(_actNumber, _cutsceneType);

            if (_thisNarrative is null)
            {
                StSDebug.LogWarning($"Parameters for act/type were not set before calling setup or data doesn't exist in the narrative manager for act {_actNumber} of type {_cutsceneType}");
                EndNarrative();
                return;
            }

            int numPanels = panels.Length;
            var currentCountThisScreen = 0;
            foreach (NarrativePanel narrativePanel in _thisNarrative.allPanels)
            {
                currentCountThisScreen += 1;

                if (narrativePanel.forceFullScreen)
                {
                    _panelsPerPage.Add(currentCountThisScreen);
                    currentCountThisScreen = 0;
                }
                else
                {
                    if (currentCountThisScreen == numPanels)
                    {
                        _panelsPerPage.Add(currentCountThisScreen);
                        currentCountThisScreen = 0;
                    }
                }
            }

            if (currentCountThisScreen > 0)
            {
                _panelsPerPage.Add(currentCountThisScreen);
            }

            _timeUntilTrigger = timeToMoveToNextPanel;
            ShowFirstPanel();
        }

        public void SetParameters(int act, NarrativeSO.NarrativeType type)
        {
            _actNumber = act;
            _cutsceneType = type;
        }

        public string GetCutsceneIDForSaveSystem()
        {
            return _actNumber + "_" + _thisNarrative?.readableNarrativeName + "_" + _cutsceneType;
        }
    }
}