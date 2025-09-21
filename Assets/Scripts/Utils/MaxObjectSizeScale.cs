using UnityEngine;

namespace Utils
{
    public class MaxObjectSizeScale  : MonoBehaviour
    {
        public RectTransform target;
        public float maxWidth = 800f;
        public float scaleFactor = 0.7f;

        private void Start()
        {
#if UNITY_STANDALONE_WIN
        // Only runs in Windows standalone builds
        ApplyScaling();
#endif
        }

        private void ApplyScaling()
        {
            if (target == null) target = GetComponent<RectTransform>();

            if (target != null)
            {
                // Example: clamp width with scaling
                var currentWidth = target.sizeDelta.x;
                if (currentWidth > maxWidth)
                {
                    target.localScale = Vector3.one * scaleFactor;
                }
            }
        }
    }
}