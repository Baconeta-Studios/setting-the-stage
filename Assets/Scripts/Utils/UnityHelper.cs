public static class UnityHelper
{

#if UNITY_EDITOR
    public static bool IsInUnityEditor = true;
    public static bool IsNotInUnityEditor = !IsInUnityEditor;
#else
    public static bool IsInUnityEditor = false;
    public static bool IsNotInUnityEditor = !IsInUnityEditor;
#endif
#if DEBUG
    public static bool IsInDebug = true;
#else
    public static bool IsInDebug = false;
#endif
}