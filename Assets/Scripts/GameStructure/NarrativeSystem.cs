using System;
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

        [SerializeField] private Button backButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button skipAllButton;
        [SerializeField] private Button[] allButtonsToPauseAutoScroll;

        [SerializeField] private float timeToMoveToNextPanel = 10f; // Time in seconds
        
        private float _timeUntilTrigger;

        private NarrativeSO _thisNarrative;
        private NarrativeDataManager _dataManagerRef;

        private Action _actionOnEnd;

        private void Start()
        {
            _timeUntilTrigger = timeToMoveToNextPanel;
            ShowPanel();
            
            // Populate data for the system from the narrative manager
            _dataManagerRef = NarrativeDataManager.Instance;
        }

        public void Update()
        {
            _timeUntilTrigger -= Time.deltaTime;

            if (_timeUntilTrigger <= 0f)
            {
                MoveToNextPage(); // A page consists of panels.size panels
            }
        }

        private void ShowPanel()
        {
            foreach (var panel in panels)
            {
                panel.gameObject.SetActive(true);
            }

            ResetTimer();
        }

        private void ResetTimer()
        {
            _timeUntilTrigger = timeToMoveToNextPanel;
        }

        public void MoveToNextPage()
        {
            ResetTimer();
            // move to next page of panels or end narrative
        }
        
        public void MoveToPreviousPage()
        {
            ResetTimer();
            // move to prev page of panels if it exists
        }

        private void OnUserInteraction()
        {
            ResetTimer();
        }

        private void OnEnable()
        {
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
        public void Setup(Action invokeOnEnd)
        {
            _actionOnEnd = invokeOnEnd;
        }

        public void SetParameters(int act, NarrativeSO.NarrativeType type)
        {
            actNumber = act;
            cutsceneType = type;
        }
    }
}