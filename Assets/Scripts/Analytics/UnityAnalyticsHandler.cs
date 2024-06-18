using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using Utils;

namespace Analytics
{
    public class UnityAnalyticsHandler : AnalyticsHandler
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

        /*** Begin Unity state functions ***/
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
        /*** End Unity state function ***/

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

        public override void RequestDataDeletion()
        {
            OptOut();
            AnalyticsService.Instance.RequestDataDeletion();
        }

        // We use a dict and convert everything to strings for the analytics system to be more generic
        public override void LogEvent(string eventName, Dictionary<string, object> values)
        {
            Dictionary<string, object> analytics = GetBaselineAnalytics().MergeDictionary(values);
            
            // Send some analytics to server or whatever we do
            InvokeAnalyticsUpdate(eventName, analytics);
        }

        // Here we get the analytics data we want to send with every analytics event
        private Dictionary<string, object> GetAnalyticsFromSave(int level_id)
        {
            Dictionary<string, object> analytics = new Dictionary<string, object>
            {
                { "total_levels_played", SaveManager.Instance.GetTotalLevelsPlayed() },
                { "times_level_was_played_before", SaveManager.Instance.GetTimesLevelWasPlayedBefore(level_id) },
                { "highscore_for_level", SaveManager.Instance.GetHighscoreForLevel(level_id) }
            };
            
            return analytics;
        }

        private void InvokeAnalyticsUpdate(string eventName, Dictionary<string, object> analytics)
        {
#if ENABLE_CLOUD_SERVICES_ANALYTICS
            UnityEngine.Analytics.Analytics.CustomEvent(eventName, analytics);
#endif
        }
    }
}