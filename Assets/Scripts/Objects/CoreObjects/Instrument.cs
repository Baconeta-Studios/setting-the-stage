using Animation;
using GameStructure;
using UnityEngine;

public class Instrument : StSObject
{
    [SerializeField] private Vector3 instrumentHoldingRotation;
    [SerializeField] private Vector3 instrumentHoldingPosition;
    [SerializeField] private float instrumentHoldingScale = 0.6f;
    [SerializeField] private SocketPosition socketPosition;
    [SerializeField] private Sprite instrumentSprite;
    [SerializeField] private string animationHoldName;
    [SerializeField] private string[] animationPlayNames;
    [SerializeField] private string selectionAudioName;
    
    [SerializeField][Tooltip("This must be unique and never change for save file reasons")]
    private string instrumentID;
    public string GetInstrumentID() => instrumentID;

    public Sprite InstrumentSprite => instrumentSprite;

    public string AnimationHoldName => animationHoldName;

    public Vector3 GetInstrumentHoldingRotation()
    {
        return instrumentHoldingRotation;
    }
    
    public Vector3 GetInstrumentHoldingPosition()
    {
        return instrumentHoldingPosition;
    }
    
    public float GetInstrumentHoldingScale()
    {
        return instrumentHoldingScale;
    }

    public SocketPosition GetSocketPosition()
    {
        return socketPosition;
    }
    
    public string GetSelectionTrackName()
    {
        return selectionAudioName;
    }
}
