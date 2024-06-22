using GameStructure;
using UnityEngine;

public class Instrument : StSObject
{
    [SerializeField] private Vector3 instrumentHoldingRotation;
    public Vector3 GetInstrumentHoldingRotation()
    {
        return instrumentHoldingRotation;
    }
}
