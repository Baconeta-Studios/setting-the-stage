using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

namespace Analytics
{
    public class UnityAnalyticsHandler : AnalyticsHandlerBase
    {

        /// Has the player granted explicit consent for data collection?
        [SerializeField] private bool _playerConsentsDebug;
        private bool _playerConsents;

        /// Starts false - when a player launches the game, we check if this is false.
        /// If it is, we will ask them if they would like to opt-out of analytics collection.
        /// Regardless, we will save to PlayerPrefs that they have been asked and never show the prompt to them again.
        private bool _consentHasBeenRequested;

        // Persistent Data Variables //
        private int _totalLevelsPlayed;
        private IDictionary<string, int> _levelPlayCount;
        private int _interactionsMadeThisAttempt;

        /*** Begin Unity state functions ***/
        private async void Init()
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

        private void LoadConsent()
        {
            #if !UNITY_EDITOR
            _playerConsentsDebug = false;
            #endif
            
            if (_playerConsentsDebug || _playerConsents)
            {
                AnalyticsService.Instance.StartDataCollection();
            }
        }

        protected void OnEnable()
        {
            Init();
        }

        protected void OnDisable()
        {
            OptOut();
        }
        /*** End Unity state function ***/

        /// Called when the player opts-in via the settings menu.
        public override void OptIn()
        {
            if (_playerConsents) return;

            _playerConsents = true;
            AnalyticsService.Instance.StartDataCollection();
        }

        public override void OptOut()
        {
            if (!_playerConsents) return;

            _playerConsents = false;
            AnalyticsService.Instance.StopDataCollection();
        }

        public override void RequestDataDeletion()
        {
            OptOut();
            AnalyticsService.Instance.RequestDataDeletion();
        }

        protected override void SendAnalytics(string eventName, Dictionary<string, object> analytics)
        {
            StSDebug.Log($"Recording analytics event {eventName} with {analytics.Count} parameters");
#if ENABLE_CLOUD_SERVICES_ANALYTICS
            CustomEvent gameEvent = new(eventName);
            foreach (KeyValuePair<string, object> kvp in analytics)
            {
                gameEvent.Add(kvp.Key, kvp.Value);
            }
            AnalyticsService.Instance.RecordEvent(gameEvent);
#endif
        }
    }
}