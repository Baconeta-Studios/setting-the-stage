using GameStructure.Narrative;
using UnityEngine;
using Utils;

namespace Managers
{
    public class NarrativeDataManager : EverlastingSingleton<NarrativeDataManager>
    {
        [SerializeField] private NarrativeSo[] allNarrativeData;
        
        public NarrativeSo GetNarrativeData(int actNumber, NarrativeSo.NarrativeType narrativeType)
        {
            foreach (NarrativeSo narrativeSo in allNarrativeData)
            {
                if (narrativeSo)
                {
                    if (narrativeSo.actNumber == actNumber && narrativeSo.narrativeType == narrativeType)
                    {
                        StSDebug.LogInfo($"Loaded {narrativeSo.readableNarrativeName} for act {actNumber}");
                        return narrativeSo;
                    }
                }
            }
            // We don't error here because we don't want to define the behaviour when data is missing
            StSDebug.LogWarning($"Narrative data doesn't exist for act {actNumber} and narrative type {narrativeType}");
            return null;
        }
    }
}