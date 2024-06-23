using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using static Utils.UnityHelper;

namespace Analytics
{
    public class UnityAnalyticsHandler : AnalyticsHandlerBase
    {
        /*** Begin Unity state functions ***/
        protected override void OnEnable()
        {
            // Initialize the analytics core first, and then check for existing player consent to start data collection.
            InitializeAnalytics();
            base.OnEnable();
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
        }
        /*** End Unity state function ***/

        /// Called when the player opts-in via the settings menu.
        public override void OptIn()
        {
            base.OptIn();
            AnalyticsService.Instance.StartDataCollection();
        }

        public override void OptOut()
        {
            base.OptOut();
            AnalyticsService.Instance.StopDataCollection();
        }

        public override void RequestDataDeletion()
        {
            base.RequestDataDeletion();
            AnalyticsService.Instance.RequestDataDeletion();
        }

        protected override void SendAnalytics(string eventName, Dictionary<string, object> analytics)
        {
            StSDebug.Log($"Recording analytics event {eventName} with {analytics.Count} parameters");

            if (DoEnableCloudServicesAnalytics) {
                CustomEvent gameEvent = new(eventName);
                foreach (KeyValuePair<string, object> kvp in analytics)
                {
                    gameEvent.Add(kvp.Key, kvp.Value);
                }
                AnalyticsService.Instance.RecordEvent(gameEvent);
            }
        }

        private async void InitializeAnalytics()
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
    }
}