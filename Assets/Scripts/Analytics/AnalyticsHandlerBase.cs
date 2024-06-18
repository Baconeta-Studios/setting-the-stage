using System.Collections.Generic;
using Utils;

namespace Analytics
{
    public abstract class AnalyticsHandlerBase : EverlastingSingleton<AnalyticsHandlerBase>
    {
        public enum EventType
        {
            LevelStartedEvent,
            StagePlacementEvent,
            LevelCompletedEvent,
            LevelAbandonedEvent,
        }

        public abstract void OptIn();
        public abstract void OptOut();
        public abstract void RequestDataDeletion();

        protected abstract void SendAnalytics(string eventName, Dictionary<string, object> analytics);
        
        /// <summary>
        /// Log an AnalyticsEvent with UnityAnalytics. This function will add to the values dictionary.
        ///
        /// We use a dict and convert everything to strings for the analytics system to be more generic
        /// </summary>
        /// <param name="eventName">name of the event being logged.</param>
        /// <param name="analytics">Key-value pairs of an analytic being recorded and its value. Should contain "level_identifier" when applicable.</param>
        public void LogEvent(string eventName, Dictionary<string, object> analytics)
        {
            analytics.TryGetValue("level_identifier", out object levelID);
            if (levelID is not null)
            {
                analytics = GetLevelAnalytics((int) levelID).MergeDictionary(analytics);
            }
            
            analytics = GetDefaultAnalytics().MergeDictionary(analytics);
            SendAnalytics(eventName, analytics);
        }

        // Here we get the analytics data we want to send with every analytics event
        private Dictionary<string, object> GetDefaultAnalytics()
        {
            Dictionary<string, object> analytics = new Dictionary<string, object>
            {
                { "total_levels_played", SaveManager.Instance.GetTotalLevelsPlayed() }
            };
            
            return analytics;
        }
        
        private Dictionary<string, object> GetLevelAnalytics(int levelID)
        {
            Dictionary<string, object> analytics = new Dictionary<string, object>
            {
                { "times_level_was_played_before", SaveManager.Instance.GetTimesLevelWasPlayedBefore(levelID) },
                { "highscore_for_level", SaveManager.Instance.GetHighscoreForLevel(levelID) }
            };
            
            return analytics;
        }
    }
}
