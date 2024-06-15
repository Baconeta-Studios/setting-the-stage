using System;
using Managers;

namespace GameStructure.Narrative
{
    public class NarrativeController
    {
        private int _actNumber;
        private NarrativeSo.NarrativeType _cutsceneType;
        private NarrativeSo _thisNarrative;
        private NarrativeDataManager _dataManagerRef;
        private NarrativeLayout _narrativeLayout;

        private Action<NarrativeController> _actionOnEnd;

        public void Setup(NarrativeLayout layout, Action<NarrativeController> invokeOnEnd)
        {
            _actionOnEnd = invokeOnEnd;
            _narrativeLayout = layout;

            // Populate data for the system from the narrative manager
            _dataManagerRef = NarrativeDataManager.Instance;
            _thisNarrative = _dataManagerRef.GetNarrativeData(_actNumber, _cutsceneType);

            if (_thisNarrative is null)
            {
                StSDebug.LogWarning($"Parameters for act/type were not set before calling setup or data doesn't exist in the narrative manager for act {_actNumber} of type {_cutsceneType}");
                EndNarrative();
                return;
            }

            // Set up layout based on data
            _narrativeLayout.Setup(_thisNarrative.allPanels, this);

            _narrativeLayout.gameObject.SetActive(true);
        }

        public void SetParameters(int act, NarrativeSo.NarrativeType type)
        {
            _actNumber = act;
            _cutsceneType = type;
        }

        public string GetCutsceneIDForSaveSystem()
        {
            return _actNumber + "_" + _thisNarrative?.readableNarrativeName + "_" + _cutsceneType;
        }

        public void EndNarrative()
        {
            _narrativeLayout.DestroySelf();
            _actionOnEnd?.Invoke(this); // could consider event triggers also but this seems ok for now
        }
    }
}