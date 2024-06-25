using System.Collections.Generic;
using Analytics;
using UnityEngine;
using Utils;

public class StSDebug : MonoBehaviour
{
    /// <summary>
    /// Log info into Debug.Log when in editor/when debug mode is on.
    /// </summary>
    /// <param name="message">The log to record.</param>
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
        } else {
            AnalyticsHandlerBase.Instance.LogEvent("LogErrorEvent", new Dictionary<string, object>
            {
                { "errorString", message }
            });
        }
    }
}
