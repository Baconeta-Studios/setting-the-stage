using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

namespace Analytics
{
    public class UnityAnalytics : EverlastingSingleton<UnityAnalytics>
    {
        private bool analytics_enabled = false;

        private int total_levels_played;
        private IDirectionary<string, int> level_play_count; 
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


        private void InitializeDefaults()
        {
            total_levels_played = 0;
            level_play_count = new IDirectionary<string, int>();
            interactions_made_this_attempt = 0;            
        }

        public void OnLevelStartedEvent()
        {
            if (level_play_count.ContainsKey(level_id)) {
                level_play_count[level_id]++;
            } else {
                level_play_count.Add(level_id, )
            }

            if (analytics_enabled) 
            {
                InvokeAnalyticsUpdate()
            }
        }

        public void OnStagePlacementEvent()
        {


            if (analytics_enabled) 
            {
                InvokeAnalyticsUpdate()
            }
        }

        public void OnLevelCompletedEvent()
        {

            
            if (analytics_enabled) 
            {
                InvokeAnalyticsUpdate()
            }
        }

        public void OnLevelAbandonedEvent(
            string level_id,
            string act_id,
        )
        {
            int total_levels_played;
            int times_played_before;
            int interactions_made_this_attempt;
            int score;


            
            if (analytics_enabled) 
            {
                InvokeAnalyticsUpdate()
            }
        }
    }
}