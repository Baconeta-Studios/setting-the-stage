using System.Collections.Generic;
using Utils;

namespace Analytics
{
    public abstract class AnalyticsHandler : EverlastingSingleton<AnalyticsHandler>
    {
        public abstract void OptIn();
        public abstract void OptOut();
        public abstract void RequestDataDeletion();
        public abstract void LogEvent(string eventName, Dictionary<string, string> values);
    }
}
