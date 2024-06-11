using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[Serializable]
public class NarrativeSO : ScriptableObject
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
    public List<NarrativePanel> allPanels;
    public NarrativeType narrativeType = NarrativeType.ActIntro;
}