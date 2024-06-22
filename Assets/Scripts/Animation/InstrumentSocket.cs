using UnityEngine;

namespace Animation
{
    public enum SocketPosition
    {
        LeftHand,
        RightHand
    }

    public class InstrumentSocket : MonoBehaviour
    {
        // We use RectTransform for now, so that we can mark the socket sprites as UI or foreground elements
        [SerializeField] private RectTransform leftHandSocket;
        [SerializeField] private RectTransform rightHandSocket;
        [SerializeField] private SpriteRenderer leftHandSprite;
        [SerializeField] private SpriteRenderer rightHandSprite;

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

            switch (instrument.GetSocketPosition())
            {
                case SocketPosition.LeftHand:
                    leftHandSprite.sprite = instrument.InstrumentSprite;
                    leftHandSocket.transform.localEulerAngles = instrument.GetInstrumentHoldingRotation();
                    leftHandSocket.transform.localPosition = instrument.GetInstrumentHoldingPosition();
                    break;
                case SocketPosition.RightHand:
                    rightHandSprite.sprite = instrument.InstrumentSprite;
                    rightHandSocket.transform.localEulerAngles = instrument.GetInstrumentHoldingRotation();
                    rightHandSocket.transform.localPosition = instrument.GetInstrumentHoldingPosition();
                    break;
                default:
                    StSDebug.LogError("Invalid socket position given.");
                    break;
            }
        }

        public void RemoveInstrumentFromSocket()
        {
            leftHandSprite.sprite = null;
            rightHandSprite.sprite = null;
        }
    }
}