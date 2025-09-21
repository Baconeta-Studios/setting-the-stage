using UnityEngine;

public class PlatformScaler : MonoBehaviour
{
    [Header("Platform Specific Scales")] public float windowsScale = 1f;
    public float iosScale = 1f;
    public float androidScale = 1f;

    [Header("Optional Aspect Ratio Check")] [Tooltip("Enable to adjust for screens wider/narrower than 16:9")]
    public bool useAspectRatioAdjustment = false;

    private void Start()
    {
        float scale = 1f;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        scale = windowsScale;
#elif UNITY_IOS
        scale = iosScale;
#elif UNITY_ANDROID
        scale = androidScale;
#endif

        if (useAspectRatioAdjustment)
        {
            float aspect = (float)Screen.width / Screen.height;
            float targetAspect = 16f / 9f;

            // If aspect ratio is wider than 16:9, shrink a bit
            if (aspect > targetAspect)
                scale *= targetAspect / aspect;
            // If aspect ratio is taller than 16:9, stretch a bit
            else if (aspect < targetAspect)
                scale *= aspect / targetAspect;
        }

        transform.localScale = Vector3.one * scale;
    }
}