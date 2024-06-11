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
        // These defined values are used when we have an explicit GO or prefab set for an act,
        // otherwise values are set in the SetParameters() call
        [SerializeField] private int actNumber;
        [SerializeField] private NarrativeSO.NarrativeType cutsceneType;

        [SerializeField] private Image[] panels;
        [SerializeField] private Image fullScreenPanel;

        [SerializeField] private Button backButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button skipAllButton;
        [SerializeField] private Button[] allButtonsToPauseAutoScroll;

        [SerializeField] private float timeToMoveToNextPanel = 10f; // Time in seconds

        private float _timeUntilTrigger;

        private NarrativeSO _thisNarrative;
        private NarrativeDataManager _dataManagerRef;
        private List<int> _panelsPerPage = new();
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
            var totalPanelsBeforeThisPage = _panelsPerPage.Take(pageOfPanels).Sum();

            if (_panelsPerPage[pageOfPanels] == 1)
            {
                // Show single panel only
                fullScreenPanel.sprite = _thisNarrative.allPanels[totalPanelsBeforeThisPage].panelImage;
                fullScreenPanel.gameObject.SetActive(true);
            }
            else // We will show as many panels are we show on screen
            {
                fullScreenPanel.gameObject.SetActive(false);
                for (int i = 0; i < panels.Length; i++)
                {
                    var thisPanelSprite = _thisNarrative.allPanels[totalPanelsBeforeThisPage + i].panelImage;
                    panels[i].sprite = thisPanelSprite;
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
            // Populate data for the system from the narrative manager
            _dataManagerRef = NarrativeDataManager.Instance;

            foreach (var button in allButtonsToPauseAutoScroll)
            {
                button.onClick.AddListener(OnUserInteraction);
            }
        }

        private void OnDisable()
        {
            foreach (var button in allButtonsToPauseAutoScroll)
            {
                button.onClick.RemoveListener(OnUserInteraction);
            }
        }

        // This system should calculate the number of screens needed to show all panels 
        public void Setup(Action<NarrativeSystem> invokeOnEnd)
        {
            _actionOnEnd = invokeOnEnd;

            _thisNarrative = _dataManagerRef.GetNarrativeData(actNumber, cutsceneType);

            var numPanels = panels.Length;
            var currentCountTotal = 0;
            var currentCountThisScreen = 0;
            foreach (var narrativePanel in _thisNarrative.allPanels)
            {
                currentCountThisScreen += 1;
                currentCountTotal += 1;

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
            actNumber = act;
            cutsceneType = type;
        }

        public string GetCutsceneIDForSaveSystem()
        {
            return actNumber + "_" + _thisNarrative.readableNarrativeName + "_" + cutsceneType;
        }
    }
}