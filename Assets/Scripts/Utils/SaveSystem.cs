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
            
            [ReadOnly]
            public int highestCompletedAct = -1;

            [ReadOnly]
            public List<string> narrativesViewed = new List<string>();

            public float GetStarsForChapter(int actNumber, int chapterNumber)
            {
                foreach (ChapterSaveData chapter in chapterSaveData)
                {
                    if (chapter.actNumber == actNumber && chapter.chapterNumber == chapterNumber)
                    {
                        return chapter.starsEarned;
                    }
                }

                return 0.0f;
            }
        }

        //The currently loaded user data.
        [SerializeField] private UserData _userData;
        private const string SaveFileName = "SettingTheStageUserSave";
        
        public UserData GetUserData()
        {
            LoadUserData();
            return _userData;
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
                    _userData = loadedData;
                    StSDebug.Log("User Data loaded successfully.");
                    break;
                case PathStatus.Empty:
                    break;
                case PathStatus.DoesNotExist:
                    StSDebug.LogWarning("User Data did not exist. Creating new save data.");
                    _userData = new UserData();
                    SaveUserData();
                    break;
            }
        }// TODO UUID

        public void ResetUserData()
        {
            if (_userData == null)
            {
                _userData = new UserData();
            }
            else
            {
                _userData.userName = "";
                _userData.chapterSaveData.Clear();
                _userData.totalStarsEarned = 0.0f;
                _userData.highestCompletedAct = -1;
                _userData.narrativesViewed.Clear();
            }
            StSDebug.LogWarning("User Data Reset!");
            SaveUserData(_userData);
        }
        #endregion
        
        protected override void Awake() 
        {
            base.Awake();
            LoadUserData();
        }

        public void ChapterCompleted(int actNumber, int chapterNumber, float starsEarned)
        {
            if (_userData == null)
            {
                LoadUserData();
                
                //if user data is still null, create a new user data.
                if (_userData == null)
                {
                    _userData = new UserData();
                    StSDebug.LogWarning($"Upon chapter completion, no save data was found. Creating a new save.");
                }
            }

            bool hasDataChanged = false;
            
            ChapterSaveData newChapterSaveData = new ChapterSaveData(actNumber, chapterNumber, starsEarned);
            if (_userData.chapterSaveData.Contains(newChapterSaveData))
            {
                int index = _userData.chapterSaveData.IndexOf(newChapterSaveData);
                ChapterSaveData storedChapterSaveData = _userData.chapterSaveData[index];
                
                // If the player has earned more stars, overwrite the save data.
                if (newChapterSaveData.starsEarned > storedChapterSaveData.starsEarned)
                {
                    // Minus the previous stars earned as we add on the new stars earned further down.
                    // E.g if we already have 1.5 stars, and earn a further 1.5 stars (total of 3) then we'll minus 1.5 and add 3 further down.
                    _userData.totalStarsEarned -= storedChapterSaveData.starsEarned;
                    
                    _userData.chapterSaveData[index] = newChapterSaveData;
                    hasDataChanged = true;
                }
                // else ignore the new data.
            }
            else // This is a new chapter - Add the save data
            {
                _userData.chapterSaveData.Add(newChapterSaveData);
                hasDataChanged = true;
            }
            
            // Accumulate our new stars in our total.
            _userData.totalStarsEarned += newChapterSaveData.starsEarned;

            if (hasDataChanged)
            {
                // Sort by Act > Chapter > Stars
                _userData.chapterSaveData.Sort();
                SaveUserData(_userData);
            }
        }

        public void ActComplete(int actNumber)
        {
            if (actNumber > _userData.highestCompletedAct)
            {
                _userData.highestCompletedAct = actNumber;
                SaveUserData(_userData);
            }
        }

        public void SetCutsceneWatched(string cutsceneID)
        {
            _userData.narrativesViewed.Add(cutsceneID);
            SaveUserData(_userData);
        }

        public bool HasSeenCutscene(string cutsceneID)
        {
            return _userData.narrativesViewed.Contains(cutsceneID);
        }
    }
}





