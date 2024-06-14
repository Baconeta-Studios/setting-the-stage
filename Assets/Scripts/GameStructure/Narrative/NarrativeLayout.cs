using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameStructure.Narrative
{
    public class NarrativeLayout : MonoBehaviour
    {
        [SerializeField] protected NarrativePanelObject[] panelObjects;
        [SerializeField] private NarrativePanelObject fullScreenPanelObject;
        [SerializeField] private GameObject textPanelPrefab;

        [SerializeField] private Button backButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button skipAllButton;
        [SerializeField] private Button[] allButtonsToPauseAutoScroll;

        [SerializeField] protected float timeToMoveToNextPanel = 10f; // Time in seconds

        protected NarrativeController NarrativeController;

        protected float TimeUntilTrigger;
        private readonly List<int> _panelsPerPage = new();
        private int _pageOnScreen = -1;
        protected List<NarrativePanelData> AllPanels;
        private List<GameObject> _currentTextPanels = new();

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

        public virtual void Update()
        {
            TimeUntilTrigger -= Time.deltaTime;

            if (TimeUntilTrigger <= 0f)
            {
                MoveToNextPage(); // A page consists of panels.Length # of panels
            }
        }

        protected void ResetTimer()
        {
            TimeUntilTrigger = timeToMoveToNextPanel;
        }

        public virtual void Setup(List<NarrativePanelData> narrativePanels, NarrativeController controller)
        {
            NarrativeController = controller;
            AllPanels = narrativePanels;

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

            TimeUntilTrigger = timeToMoveToNextPanel;
            ShowFirstPanel();
        }

        private void ShowFirstPanel()
        {
            backButton.interactable = false;
            ShowPage(0);
        }

        private void ShowPage(int pageOfPanels)
        {
            // First we purge all text panels
            PurgeAllTextPanelsOffScreen();
            
            // Count the total number of panels that come before this page
            int totalPanelsBeforeThisPage = _panelsPerPage.Take(pageOfPanels).Sum();

            if (_panelsPerPage[pageOfPanels] == 1)
            {
                SetupPanel(fullScreenPanelObject, fullScreenPanelObject.NarrativePanelImage, AllPanels[totalPanelsBeforeThisPage]);
                foreach (NarrativePanelObject panelObject in panelObjects)
                {
                    panelObject.SetActive(false);
                }
            }
            else // We will populate each panel on the page
            {
                fullScreenPanelObject.SetActive(false);
                for (var i = 0; i < panelObjects.Length; i++)
                {
                    SetupPanel(panelObjects[i], panelObjects[i].NarrativePanelImage, AllPanels[totalPanelsBeforeThisPage + i]);
                }
            }

            _pageOnScreen = pageOfPanels;
        }

        private void PurgeAllTextPanelsOffScreen()
        {
            foreach (GameObject textPanel in _currentTextPanels)
            {
                Destroy(textPanel?.gameObject);
            }

            _currentTextPanels = new List<GameObject>();   // clear any null refs
        }

        public void MoveToNextPage()
        {
            ResetTimer();

            // move to next page of panels or end narrative
            if (_pageOnScreen + 1 >= _panelsPerPage.Count)
            {
                NarrativeController.EndNarrative();
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

        protected void SetupPanel(NarrativePanelObject panelObject, Image panelImage,  NarrativePanelData panelData)
        {
            panelImage.sprite = panelData.panelImage;

            // Setup any text elements
            for (var i = 0; i < panelData.textPanels?.Count; i++)
            {
                NarrativeTextData textData = panelData.textPanels[i];
                GameObject textPanelObject = Instantiate(textPanelPrefab, panelObject.transform);
                SetTextData(textPanelObject, textData);
                SetPanelData(textPanelObject, textData);
                _currentTextPanels.Add(textPanelObject);
            }

            panelObject.SetActive(true);
        }

        private static void SetPanelData(GameObject textPanelObject, NarrativeTextData textData)
        {
            textPanelObject.GetComponent<RectTransform>().sizeDelta = textData.textPanelSize;
            textPanelObject.transform.localPosition = textData.textPanelPosition;
        }

        private static void SetTextData(GameObject textPanelObject, NarrativeTextData textDataData)
        {
            TMP_Text textObject = textPanelObject.GetComponent<TMP_Text>();
            textObject.text = textDataData.text;
            textObject.fontSize = textDataData.textSize;
            textObject.font = textDataData.font;
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