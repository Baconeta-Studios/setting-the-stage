using Analytics;
using UnityEngine;
using UnityEngine.UI;

namespace Settings
{
    public class ToggleAnalytics : MonoBehaviour
    {
        [SerializeField] private Image toggleImage;
        [SerializeField] private Sprite toggleOff;
        [SerializeField] private Sprite toggleOn;
        [SerializeField] private Toggle toggle;
        [SerializeField] private AnalyticsHandlerBase analyticsHandler;

        private void OnEnable()
        {
            if (analyticsHandler == null)
            {
                // Try to remedy the missing reference to the analytics handler.
                AnalyticsHandlerBase foundHandler = FindObjectOfType<AnalyticsHandlerBase>();
                if (foundHandler != null)
                {
                    analyticsHandler = foundHandler;
                } else {
                    StSDebug.LogError("Analytics handler ref not set on toggle element, and could not be found manually.");
                    return;
                }
            }

            // Set the initial value of the toggle switch.
            bool optedIn = AnalyticsHandlerBase.GetAnalyticsState();
            OnValueChanged(optedIn);
        }

        public void OnValueChanged(bool value)
        {
            switch (value)
            {
                case true:
                    toggleImage.sprite = toggleOn;
                    analyticsHandler.OptIn();
                    break;
                case false:
                    toggleImage.sprite = toggleOff;
                    analyticsHandler.OptOut();
                    break;
            }
        }
    }
}