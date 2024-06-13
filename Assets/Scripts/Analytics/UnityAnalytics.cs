using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

namespace Analytics
{
    public class UnityAnalytics : MonoBehaviour
    {
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
        }


        private void InitializeDefaults()
        {
        }

        public void OnLevelStartedEvent()
        {
        }

        public void OnStagePlacementEvent()
        {
        }

        public void OnLevelCompletedEvent()
        {
        }

        public void OnLevelAbandonedEvent(
            string level_id,
            string act_id,
        )
        {
            }
        }
    }
}