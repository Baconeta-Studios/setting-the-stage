using System;
using UnityEngine;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Utils;

namespace Analytics
{
    public class SaveManager : EverlastingSingleton<SaveManager>
    {
        public void CreateNewSave()
        {
            string player_uuid = Guid.NewGuid().ToString();
            PlayerPrefs.SetString("Player_UUID", player_uuid);
            
        }

        public void DeleteSave()
        {

        }

        public void ResetAllAchievements()
        {
            
        }


    }
}
