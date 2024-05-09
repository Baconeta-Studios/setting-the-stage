using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StSDebug : MonoBehaviour
{
    public static void Log(string message)
    {
#if UNITY_EDITOR || DEBUG
        Debug.Log(message);
#endif
    }
    
    public static void LogWarning(string message)
    {
#if UNITY_EDITOR || DEBUG
        Debug.LogWarning(message);
#endif
    }
    
    public static void LogError(string message)
    {
#if UNITY_EDITOR || DEBUG
        Debug.LogError(message);
#endif
    }
}
