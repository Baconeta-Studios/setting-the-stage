using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameStructure.Narrative
{
    [CreateAssetMenu]
    [Serializable]
    public class NarrativeSo : ScriptableObject
    {
        public enum NarrativeType
        {
            Tutorial,
            ActIntro,
            ActOutro,
            Override
        }

        public string readableNarrativeName;
        public int actNumber;
        public List<NarrativePanelData> allPanels;
        public NarrativeType narrativeType = NarrativeType.ActIntro;
    }
}