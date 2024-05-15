using System;
using CustomEditor;
using UnityEngine;

namespace Utils
{
    public class SaveSystem : Singleton<SaveSystem>
    {
        #region UserData Management
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
        }
        
        //The currently loaded user data.
        [SerializeField] private UserData userData;
        private const string SaveFileName = "SettingTheStageUserSave";
        
        public UserData GetUserData()
        {
            return userData;
        }

        public void SaveUserData(UserData data)
        {
            string jsonData = JsonUtility.ToJson(data);
            FileUtils.WriteToFile(SaveFileName, jsonData);
            StSDebug.Log("User Data Saved");
        }

        public void LoadUserData()
        {
            PathStatus fileStatus = FileUtils.ReadFromFile(SaveFileName, out string jsonData);

            switch (fileStatus)
            {
                case PathStatus.ContainsData:
                    UserData loadedData = JsonUtility.FromJson<UserData>(jsonData);
                    userData = loadedData;
                    StSDebug.Log("User Data loaded successfully.");
                    break;
                case PathStatus.Empty:
                    break;
                case PathStatus.DoesNotExist:
                    StSDebug.LogWarning("User Data did not exist. Creating new save data.");
                    userData = new UserData();
                    break;
            }
        }

        public void ResetUserData()
        {
            if (userData == null)
            {
                userData = new UserData();
            }
            else
            {
                userData.userName = "";
                userData.currentAct = -1;
                userData.currentChapter = -1;
            }
            StSDebug.LogWarning("User Data Reset!");
            SaveUserData(userData);
        }
        #endregion
        
        protected override void Awake() 
        {
            base.Awake();
            LoadUserData();
        }

        public void ChapterCompleted(int actNumber, int chapterNumber)
        {
            if (userData == null)
            {
                LoadUserData();
                
                //if user data is still null, create a new user data.
                if (userData == null)
                {
                    userData = new UserData();
                    StSDebug.LogWarning($"Upon chapter completion, no save data was found. Creating a new save.");
                }
            }
            
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





