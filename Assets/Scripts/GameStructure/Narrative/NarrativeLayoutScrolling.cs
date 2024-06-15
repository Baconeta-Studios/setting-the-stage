using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameStructure.Narrative
{
    public class NarrativeLayoutScrolling : NarrativeLayout, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform contentBox;
        [SerializeField] private GameObject panelTemplate;
        [SerializeField] private bool useAutoScrolling = true;

        private bool _currentlyScrolling;
        private float _elapsedTime;
        private float _scrollSpeed;

        // Note: Scrolling layout system ignores forceForceFullScreen flag since all panels show at once 
        public override void Setup(List<NarrativePanelData> narrativePanels, NarrativeController controller)
        {
            NarrativeController = controller;
            AllPanels = narrativePanels;

            TimeUntilTrigger = timeToMoveToNextPanel;
            ShowAllPanels();
        }

        private void ShowAllPanels()
        {
            foreach (NarrativePanelData narrativePanelData in AllPanels)
            {
                GameObject newPanel = Instantiate(panelTemplate, contentBox);
                NarrativePanelObject narrativePanelObject = newPanel.GetComponent<NarrativePanelObject>();
                if (narrativePanelObject is null)
                {
                    StSDebug.LogError($"Missing narrative panel object on prefab {panelTemplate.name}");
                    return;
                }

                SetupPanel(narrativePanelObject, narrativePanelObject.NarrativePanelImage, narrativePanelData);
            }

            scrollRect.normalizedPosition = new Vector2(0, 0);
        }

        public override void Update()
        {
            if (!useAutoScrolling)
            {
                return;
            }

            if (_currentlyScrolling)
            {
                Scroll();
                return;
            }

            TimeUntilTrigger -= Time.deltaTime;

            if (TimeUntilTrigger <= 0f)
            {
                StartAutoScrolling();
            }
        }

        private void Scroll()
        {
            float newScrollPosition = Time.deltaTime * _scrollSpeed / contentBox.rect.width;
            scrollRect.horizontalNormalizedPosition += newScrollPosition;
        }

        private float GetScrollingSpeed()
        {
            // Returns how many pixels we should be scrolling per second
            float totalScrollTime = AllPanels.Count * timeToMoveToNextPanel;
            float contentWidth = contentBox.rect.width;
            return contentWidth / totalScrollTime;
        }

        private void StartAutoScrolling()
        {
            if (useAutoScrolling)
            {
                // Calculate the total time to scroll all panels
                _scrollSpeed = GetScrollingSpeed();
                _currentlyScrolling = true;
            }
        }

        private void StopAutoScroll()
        {
            _currentlyScrolling = false;
            ResetTimer();
        }

        public void OnScrollValueChanged(Vector2 position)
        {
            // Programmatically trigger the OnBeginDrag event
            PointerEventData eventData = new(EventSystem.current);
            OnBeginDrag(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && _currentlyScrolling)
            {
                StopAutoScroll();
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            ResetTimer();
        }
    }
}