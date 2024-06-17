using System;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Analytics;
using Utils;

namespace Analytics
{
    public class UnityAnalytics : AnalyticsHandler
    {
        /*
         * Has the player granted explicit consent for data collection?
         */
        private bool player_consents;

        /*
         * Starts false - when a player launches the game, we check if this is false.
         * If it is, we will ask them if they would like to opt-out of analytics collection.
         * Regardless, we will save to PlayerPrefs that they have been asked and never show
         *     the prompt to them again.
         */
        private bool consent_has_been_requested;

        // Persistent Data Variables //
        private int total_levels_played;
        private IDictionary<string, int> level_play_count;
        private int interactions_made_this_attempt;

        private async void Awake()
        {
            try
            {
                await UnityServices.InitializeAsync();
            }
            catch (ConsentCheckException e)
            {
                Debug.Log(e.ToString());
            }

            LoadConsent();
        }

        private async void OnEnable()
        {
            if (player_consents)
            {
                OptIn();
            }
        }

        private void OnDisable()
        {
            OptOut();
        }

        // Called when the player opts-in via the settings menu.
        public override void OptIn()
        {
            if (player_consents) return;

            player_consents = true;
            AnalyticsService.Instance.StartDataCollection();
        }

        public override void OptOut()
        {
            if (!player_consents) return;

            player_consents = false;
            AnalyticsService.Instance.StopDataCollection();
        }

        public void RequestDeletion()
        {
            OptOut();
            AnalyticsService.Instance.RequestDataDeletion();
        }

        // We use a dict and convert everything to strings for the analytics system to be more generic
        public override void LogEvent(string eventName, Dictionary<string, string> values)
        {
            Dictionary<string, string> analytics = GetBaselineAnalytics().MergeDictionary(values);
            
            // Send some analytics to server or whatever we do
            InvokeAnalyticsUpdate(eventName, analytics);
        }

        // Here we get the analytics data we want to send with every analytics event
        private Dictionary<string, string> GetDefaultAnalytics()
        {
            var defaults = new Dictionary<string, string>();
            
            defaults.Add("total_levels_played", total_levels_played.ToString());
            
            return defaults;
        }

        private void InvokeAnalyticsUpdate(string eventName, Dictionary<string, string> analytics)
        {
#if ENABLE_CLOUD_SERVICES_ANALYTICS
            Analytics.CustomEvent(eventName, analytics);
#endif
        }
    }
}