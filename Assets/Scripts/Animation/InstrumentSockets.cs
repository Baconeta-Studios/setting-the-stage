using System;
using System.Collections.Generic;
using UnityEngine;

namespace Animation
{
    public enum SocketPosition
    {
        LeftHand,
        RightHand,
        InFront
    }

    [Serializable]
    public struct SocketReferences
    {
        public SocketPosition socketPosition;
        public RectTransform socketRectTransform;
        public SpriteRenderer socketSpriteRenderer;
    }

    public class InstrumentSockets : MonoBehaviour
    {
        [SerializeField] private List<SocketReferences> allSocketReferences;

        // Here we pass in values that the instrument prefabs will hold telling a player how their socket
        // must be rotated to support the specific instrument. We elsewhere call the AnimController to tell
        // the musician which animation they must use in order to hold said instrument in the first place
        public void HoldInstrument(Instrument instrument)
        {
            if (instrument == null)
            {
                StSDebug.LogWarning("Instrument sprite null or invalid.");
                return;
            }
            
            SocketReferences socket = GetSocketRefsForInstrument(instrument);
            try
            {
                float scale = instrument.GetInstrumentHoldingScale();
                socket.socketSpriteRenderer.sprite = instrument.InstrumentSprite;
                socket.socketRectTransform.transform.localEulerAngles = instrument.GetInstrumentHoldingRotation();
                socket.socketRectTransform.transform.localPosition = instrument.GetInstrumentHoldingPosition();
                socket.socketRectTransform.transform.localScale = new Vector3(scale,scale,scale);
            }
            catch (NullReferenceException)
            {
                StSDebug.LogError($"A socket reference is missing on { socket.socketPosition }");
            }
        }

        private SocketReferences GetSocketRefsForInstrument(Instrument instrument)
        {
            foreach (SocketReferences socket in allSocketReferences)
            {
                if (socket.socketPosition == instrument.GetSocketPosition())
                {
                    return socket;
                }
            }
            StSDebug.LogError("Invalid socket position given or socket not found on this musician.");
            return new SocketReferences();
        }

        public void RemoveInstrumentFromSocket()
        {
            foreach (SocketReferences socket in allSocketReferences)
            {
                socket.socketSpriteRenderer.sprite = null;
            }
        }
    }
}