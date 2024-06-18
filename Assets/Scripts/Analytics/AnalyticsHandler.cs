using System.Collections.Generic;
using Utils;

namespace Analytics
{
    public abstract class AnalyticsHandler : EverlastingSingleton<AnalyticsHandler>
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
        public abstract void LogEvent(EventType type, Dictionary<string, object> values);
    }
}
