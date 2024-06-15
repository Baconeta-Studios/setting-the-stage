using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using System;

namespace Analytics
{
    public class SaveManager : EverlastingSingleton<UnityAnalytics>
    {
        public void CreateNewSave()
        {
            string player_uuid = Guid.NewGuid();
            PlayerPrefs.SetString('Player_UUID', player_uuid);
            
        }

        public void DeleteSave()
        {

        }

        public void ResetAllAchievements()
        {
            
        }


    }
}
