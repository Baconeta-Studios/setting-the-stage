using System;
using CustomEditor;
using UnityEngine;

namespace Utils
{
    public class SaveSystem : Singleton<SaveSystem>
    {
        [Serializable]
        public class UserData
        {
            // Add desired save data here.
            [ReadOnly]
            public string userName;

            // The current act the player is up to
            [ReadOnly]
            public int currentAct;
            
            // The current chapter the player is up to
            [ReadOnly]
            public int currentChapter;
            
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
            StSDebug.Log("User Data Saved");
        }

        public void LoadUserData()
        {
            string jsonData = FileUtils.ReadFromFile("SettingTheStageUserSave");

            UserData loadedData = JsonUtility.FromJson<UserData>(jsonData);

            userData = loadedData;
            
            StSDebug.Log("User Data Loaded");
        }

        public void ResetUserData()
        {
            userData.userName = "";
            userData.currentAct = -1;
            userData.currentChapter = -1;
            StSDebug.LogWarning("User Data Reset!");
            SaveUserData(userData);
        }

        protected override void Awake() 
        {
            base.Awake();
            LoadUserData();
        }

        public void ChapterCompleted(int actNumber, int chapterNumber)
        {
            if (userData.currentAct < actNumber)
            {
                userData.currentAct = actNumber;

                if (userData.currentChapter < chapterNumber)
                {
                    userData.currentChapter = chapterNumber;
                }
                
                SaveUserData(userData);
            }
        }
    }
}





