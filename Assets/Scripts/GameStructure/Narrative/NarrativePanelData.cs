using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameStructure.Narrative
{
    public struct NarrativeText
    {
        public string Text;
        public Vector2 TextPanelSize;
        public Vector2 TextPanelPosition;
        public int TextSize;
        public Font Font;
        public int DisplayDisplayInMs;
    }
    
    [CreateAssetMenu]
    [Serializable]
    public class NarrativePanelData : ScriptableObject
    {
        // Contains the information about a specific panel for displaying story information on the screen
        public string simplePanelName;
        public bool forceFullScreen; // Use this to force it to be the only element on screen regardless of NarrativeSO settings
        public Sprite panelImage; // if this is null we use the default layout for panels
        public List<NarrativeText> TextPanels;
    }
}