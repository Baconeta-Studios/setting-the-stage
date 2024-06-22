using System;
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
        // We use RectTransform so we can mark the socket sprites as UI or foreground elements
        [SerializeField] private RectTransform leftHandSocket;
        [SerializeField] private RectTransform rightHandSocket;
        [SerializeField] private SpriteRenderer leftHandSprite;
        [SerializeField] private SpriteRenderer rightHandSprite;

        public void HoldInstrument(Sprite instrument, Transform instrumentTransform, SocketPosition socket)
        {
            if (instrument == null)
            {
                StSDebug.LogWarning("Instument sprite null or invalid.");
                return;
            }

            switch (socket)
            {
                case SocketPosition.LeftHand:
                    leftHandSprite.sprite = instrument;
                    leftHandSocket.transform.rotation = instrumentTransform.rotation;
                    leftHandSocket.transform.localPosition = instrumentTransform.localPosition;
                    leftHandSocket.transform.localScale = instrumentTransform.localScale;
                    break;
                case SocketPosition.RightHand:
                    rightHandSprite.sprite = instrument;
                    rightHandSocket.transform.rotation = instrumentTransform.rotation;
                    rightHandSocket.transform.localPosition = instrumentTransform.localPosition;
                    rightHandSocket.transform.localScale = instrumentTransform.localScale;
                    break;
                default:
                    StSDebug.LogError("Invalid socket position given.");
                    break;
            }
        }
    }
}