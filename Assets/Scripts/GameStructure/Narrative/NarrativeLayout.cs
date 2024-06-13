using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace GameStructure.Narrative
{
    public class NarrativeLayout : MonoBehaviour
    {
        [SerializeField] private Image[] panels;
        [SerializeField] private Image fullScreenPanel;

        [SerializeField] private Button backButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button skipAllButton;
        [SerializeField] private Button[] allButtonsToPauseAutoScroll;

        [SerializeField] private float timeToMoveToNextPanel = 10f; // Time in seconds

        private NarrativeController _narrativeController;

        private float _timeUntilTrigger;
        private readonly List<int> _panelsPerPage = new();
        private int _pageOnScreen = -1;
        private List<NarrativePanel> _allPanels;

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
                fullScreenPanel.sprite = _allPanels[totalPanelsBeforeThisPage].panelImage;
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
                    Sprite thisPanelSprite = _allPanels[totalPanelsBeforeThisPage + i].panelImage;
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
                _narrativeController.EndNarrative();
            }
            else
            {
                ShowPage(_pageOnScreen + 1);
            }

            backButton.interactable = true;
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
        public void Setup(List<NarrativePanel> narrativePanels, NarrativeController controller)
        {
            _narrativeController = controller;
            _allPanels = narrativePanels;

            int numPanels = panels.Length;
            var currentCountThisScreen = 0;
            foreach (NarrativePanel narrativePanel in narrativePanels)
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

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}