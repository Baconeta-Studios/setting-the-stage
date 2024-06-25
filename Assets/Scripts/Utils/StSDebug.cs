using System.Collections;
using System.Collections.Generic;
using Analytics;
using UnityEngine;
using Utils;

public class StSDebug : MonoBehaviour
{
    public static void Log(string message)
    {
        if (UnityHelper.IsInUnityEditor || UnityHelper.IsInDebug)
        {
            Debug.Log(message);
        }
    }
    
    public static void LogWarning(string message)
    {
        if (UnityHelper.IsInUnityEditor || UnityHelper.IsInDebug)
        {
            Debug.LogWarning(message);
        }
    }
    
    public static void LogError(string message)
    {
        if (UnityHelper.IsInUnityEditor || UnityHelper.IsInDebug)
        {
            Debug.LogError(message);
        }
        
        AnalyticsHandlerBase.Instance.LogEvent("LogErrorEvent", new Dictionary<string, object>
        {
            { "errorString", message }
        });
    }
}
