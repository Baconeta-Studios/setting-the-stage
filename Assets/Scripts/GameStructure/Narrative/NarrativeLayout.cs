using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GameStructure.Narrative
{
    public class NarrativeLayout : MonoBehaviour
    {
        [SerializeField] private NarrativePanelObject[] panelObjects;
        [SerializeField] private NarrativePanelObject fullScreenPanelObject;

        [SerializeField] private Button backButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button skipAllButton;
        [SerializeField] private Button[] allButtonsToPauseAutoScroll;

        [SerializeField] private float timeToMoveToNextPanel = 10f; // Time in seconds

        private NarrativeController _narrativeController;

        private float _timeUntilTrigger;
        private readonly List<int> _panelsPerPage = new();
        private int _pageOnScreen = -1;
        private List<NarrativePanelData> _allPanels;

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

        public void Update()
        {
            _timeUntilTrigger -= Time.deltaTime;

            if (_timeUntilTrigger <= 0f)
            {
                MoveToNextPage(); // A page consists of panels.Length # of panels
            }
        }

        private void ResetTimer()
        {
            _timeUntilTrigger = timeToMoveToNextPanel;
        }

        public void Setup(List<NarrativePanelData> narrativePanels, NarrativeController controller)
        {
            _narrativeController = controller;
            _allPanels = narrativePanels;

            int numPanels = panelObjects.Length;
            var currentCountThisScreen = 0;
            foreach (NarrativePanelData narrativePanel in narrativePanels)
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

        private void ShowFirstPanel()
        {
            backButton.interactable = false;
            ShowPage(0);
        }

        private void ShowPage(int pageOfPanels)
        {
            // Count the total number of panels that come before this page
            int totalPanelsBeforeThisPage = _panelsPerPage.Take(pageOfPanels).Sum();

            if (_panelsPerPage[pageOfPanels] == 1)
            {
                SetupPanel(fullScreenPanelObject, fullScreenPanelObject.NarrativePanelImage, _allPanels[totalPanelsBeforeThisPage]);
                foreach (NarrativePanelObject panelObject in panelObjects)
                {
                    panelObject.SetActive(true);
                }
            }
            else // We will populate each panel on the page
            {
                fullScreenPanelObject.SetActive(false);
                for (var i = 0; i < panelObjects.Length; i++)
                {
                    SetupPanel(panelObjects[i], panelObjects[i].NarrativePanelImage, _allPanels[totalPanelsBeforeThisPage + i]);
                }
            }

            _pageOnScreen = pageOfPanels;
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

        private void SetupPanel(NarrativePanelObject panelObject, Image panelImage,  NarrativePanelData panelDataData)
        {
            panelImage.sprite = panelDataData.panelImage;
            
            panelObject.SetActive(true);
        }

        private void OnUserInteraction()
        {
            ResetTimer();
        }
        
        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}