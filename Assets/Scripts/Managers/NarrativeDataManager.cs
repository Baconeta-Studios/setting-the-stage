using UnityEngine;
using Utils;

namespace Managers
{
    public class NarrativeDataManager : Singleton<NarrativeDataManager>
    {
        [SerializeField] private NarrativeSO[] allNarrativeData;
        
        public NarrativeSO GetNarrativeData(int actNumber, NarrativeSO.NarrativeType narrativeType)
        {
            foreach (NarrativeSO narrativeSo in allNarrativeData)
            {
                if (narrativeSo.actNumber == actNumber && narrativeSo.narrativeType == narrativeType)
                {
                    StSDebug.Log($"Loaded {narrativeSo.readableNarrativeName} for act {actNumber}");
                    return narrativeSo;
                }
            }
            // We don't error here because we don't want to define the behaviour when data is missing
            StSDebug.LogWarning($"Narrative data doesn't exist for act {actNumber} and narrative type {narrativeType}");
            return null;
        }
    }
}