using System.Collections.Generic;
using UnityEngine;

public class NarrativeSO : ScriptableObject
{
    public enum NarrativeType
    {
        Tutorial,
        ActIntro,
        ActOutro
    }

    public string readableNarrativeName;
    public int actNumber;
    public List<NarrativePanel> allPanels;
    public NarrativeType narrativeType = NarrativeType.ActIntro;
}