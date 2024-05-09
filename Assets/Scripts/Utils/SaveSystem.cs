using System;
using UnityEngine;

namespace Utils
{
    public class SaveSystem : MonoBehaviour
    {
        [Serializable]
        public class UserData
        {
            // Add desired save data here.
            public string userName;

            public UserData(string userName)
            {
                this.userName = userName;
            }
        }
        
        //The currently loaded user data.
        [SerializeField] private UserData userData;
        
        public UserData GetUserData()
        {
            return userData;
        }

        public void SaveUserData(UserData data)
        {
            string jsonData = JsonUtility.ToJson(data);
            FileUtils.WriteToFile("SettingTheStageUserSave", jsonData);
            StSDebug.Log("Game Saved");
        }

        public void LoadUserData()
        {
            string jsonData = FileUtils.ReadFromFile("SettingTheStageUserSave");

            UserData loadedData = JsonUtility.FromJson<UserData>(jsonData);

            userData = loadedData;
            
            StSDebug.Log("Game Loaded");

        }
    }
}





