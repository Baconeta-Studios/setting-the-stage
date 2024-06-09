using UnityEngine;

public class NarrativePanel : ScriptableObject
{
    // Contains the information about a specific panel for displaying story information on the screen
    public string simplePanelName;
    public bool forceFullScreen; // Use this to force it to be the only element on screen regardless of NarrativeSO settings
    public Sprite panelImage; // if this is null we use the default layout for panels
}