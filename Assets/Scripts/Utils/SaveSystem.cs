using System;
using System.Collections.Generic;
using CustomEditor;
using UnityEngine;

namespace Utils
{
    public class SaveSystem : Singleton<SaveSystem>
    {
        #region UserData Management

        [Serializable]
        public struct ChapterSaveData : IComparable<ChapterSaveData>
        {
            public int actNumber;
            public int chapterNumber;
            public float starsEarned;

            public ChapterSaveData(int act, int chapter, float stars) 
            {
                actNumber = act;
                chapterNumber = chapter;
                starsEarned = stars;
            }

            // This is used in List.Sort and will sort list items by Act, then Chapter, then Stars (Realistically it should never get to stars)
            public int CompareTo(ChapterSaveData other)
            {
                // Is the act higher?
                int actComparison = actNumber.CompareTo(other.actNumber);
                if (actComparison != 0)
                {
                    return actComparison;
                }

                // Is the chapter higher?
                int chapterComparison = chapterNumber.CompareTo(other.chapterNumber);
                if (chapterComparison != 0)
                {
                    return chapterComparison;
                }

                // Else return if the Stars are higher.
                return starsEarned.CompareTo(other.starsEarned);
            }
            
            public override bool Equals(object obj)
            {
                if (obj is ChapterSaveData other)
                {
                    return actNumber == other.actNumber && chapterNumber == other.chapterNumber;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return actNumber.GetHashCode() + chapterNumber.GetHashCode() + starsEarned.GetHashCode();
            }
            
        }
        
        [Serializable]
        public class UserData
        {
            // Add desired save data here.
            [ReadOnly]
            public string userName;

            [ReadOnly]
            public List<ChapterSaveData> chapterSaveData = new List<ChapterSaveData>();

            [ReadOnly]
            public float totalStarsEarned = 0.0f;
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
                userData.chapterSaveData.Clear();
                userData.totalStarsEarned = 0.0f;
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

        public void ChapterCompleted(int actNumber, int chapterNumber, float starsEarned)
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

            bool hasDataChanged = false;
            
            ChapterSaveData newChapterSaveData = new ChapterSaveData(actNumber, chapterNumber, starsEarned);
            if (userData.chapterSaveData.Contains(newChapterSaveData))
            {
                int index = userData.chapterSaveData.IndexOf(newChapterSaveData);
                ChapterSaveData storedChapterSaveData = userData.chapterSaveData[index];
                
                // If the player has earned more stars, overwrite the save data.
                if (newChapterSaveData.starsEarned > storedChapterSaveData.starsEarned)
                {
                    // Minus the previous stars earned as we add on the new stars earned further down.
                    // E.g if we already have 1.5 stars, and earn a further 1.5 stars (total of 3) then we'll minus 1.5 and add 3 further down.
                    userData.totalStarsEarned -= storedChapterSaveData.starsEarned;
                    
                    userData.chapterSaveData[index] = newChapterSaveData;
                    hasDataChanged = true;
                }
                // else ignore the new data.
            }
            else // This is a new chapter - Add the save data
            {
                userData.chapterSaveData.Add(newChapterSaveData);
                hasDataChanged = true;
            }
            
            // Accumulate our new stars in our total.
            userData.totalStarsEarned += newChapterSaveData.starsEarned;

            if (hasDataChanged)
            {
                // Sort by Act > Chapter > Stars
                userData.chapterSaveData.Sort();
                SaveUserData(userData);
            }
        }
    }
}





