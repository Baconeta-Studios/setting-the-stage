using System;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Analytics;
using Utils;

namespace Analytics
{
    public class UnityAnalytics : EverlastingSingleton<UnityAnalytics>
    {
        private bool analytics_enabled = false;

        private int total_levels_played;
        private IDictionary<string, int> level_play_count;
        private int interactions_made_this_attempt;

        private async void Start()
        {
            try
            {
                await UnityServices.InitializeAsync();
            }
            catch (ConsentCheckException e)
            {
                Debug.Log(e.ToString());
            }
        }

        private void EnableAnalytics()
        {
            if (analytics_enabled) return;

            analytics_enabled = true;
        }

        private void OnDisable()
        {
            DisableAnalytics();
        }

        private void DisableAnalytics()
        {
            analytics_enabled = false;
        }

        // Here we get the analytics data we want to send with every analytics event
        private Dictionary<string, string> GetDefaultAnalytics()
        {
            var defaults = new Dictionary<string, string>();
            
            defaults.Add("total_levels_played", total_levels_played.ToString());
            
            return defaults;
        }

        // We use a dict and convert everything to strings for the analytics system to be more generic
        public void SendAnalytics(string eventName, Dictionary<string, string> values)
        {
            Dictionary<string, string> analytics = GetDefaultAnalytics().MergeDictionary(values);
            
            // Send some analytics to server or whatever we do
            InvokeAnalyticsUpdate(eventName, analytics);
        }

        private void InvokeAnalyticsUpdate(string eventName, Dictionary<string, string> analytics)
        {
#if ENABLE_CLOUD_SERVICES_ANALYTICS
            Analytics.CustomEvent(eventName, analytics);
#endif
        }
    }
}