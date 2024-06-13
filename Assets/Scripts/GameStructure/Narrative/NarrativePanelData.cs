using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameStructure.Narrative
{
    [Serializable]
    public struct NarrativeTextData
    {
        public string text;
        public Vector2 textPanelSize;
        public Vector2 textPanelPosition;
        public int textSize;
        public TMP_FontAsset font;
        public int displayDisplayInMs;
    }
    
    [CreateAssetMenu]
    [Serializable]
    public class NarrativePanelData : ScriptableObject
    {
        // Contains the information about a specific panel for displaying story information on the screen
        public string simplePanelName;
        public bool forceFullScreen; // Use this to force it to be the only element on screen regardless of NarrativeSO settings
        public Sprite panelImage; // if this is null we use the default layout for panels
        public List<NarrativeTextData> textPanels;
    }
}