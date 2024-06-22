using Animation;
using GameStructure;
using UnityEngine;

public class Instrument : StSObject
{
    [SerializeField] private Vector3 instrumentHoldingRotation;
    [SerializeField] private Vector3 instrumentHoldingPosition;
    [SerializeField] private SocketPosition socketPosition;
    [SerializeField] private Sprite instrumentSprite;

    public Sprite InstrumentSprite => instrumentSprite;

    public Vector3 GetInstrumentHoldingRotation()
    {
        return instrumentHoldingRotation;
    }
    
    public Vector3 GetInstrumentHoldingPosition()
    {
        return instrumentHoldingPosition;
    }

    public SocketPosition GetSocketPosition()
    {
        return socketPosition;
    }
}
