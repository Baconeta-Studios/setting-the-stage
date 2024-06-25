using System;
using System.Collections.Generic;
using System.Linq;
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
            public int timesStarted;
            public int timesCompleted;

            public ChapterSaveData(int act, int chapter, float stars, int numStarts, int numCompletes) 
            {
                actNumber = act;
                chapterNumber = chapter;
                starsEarned = stars;
                timesStarted = numStarts;
                timesCompleted = numCompletes;
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
        public struct ChapterPlaysData
        {
            public int act;
            public int chapter;
            public int playsStarted;
            public int playsCompleted;
        }

        [Serializable]
        public class UserData
        {
            // Add desired save data here.
            [ReadOnly]
            public string userName;

            [ReadOnly]
            protected string playerUUID = null;

            [ReadOnly]
            public List<ChapterSaveData> chapterSaveData = new List<ChapterSaveData>();

            [ReadOnly]
            public float totalStarsEarned = 0.0f;
            
            [ReadOnly]
            public int highestCompletedAct = -1;

            [ReadOnly]
            public List<string> narrativesViewed = new List<string>();
            
            // Get user data functions
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

            /// <summary>
            /// Gets the chapter save data if it exists, or creates it freshly and adds it, if it does not. 
            /// </summary>
            /// <param name="act"> act number</param>
            /// <param name="chapter"> chapter number</param>
            /// <param name="index"> the index of the csd</param>
            /// <returns> The chapter save data object from the user data</returns>
            public ChapterSaveData GetChapterSaveData(int act, int chapter, out int index)
            {
                for (var i = 0; i < chapterSaveData.Count; i++)
                {
                    ChapterSaveData csd = chapterSaveData[i];
                    if (csd.actNumber == act && csd.chapterNumber == chapter)
                    {
                        index = i;
                        return csd;
                    }
                }
                ChapterSaveData newCsd = new(act, chapter, 0, 0, 0);
                index = chapterSaveData.Count;
                chapterSaveData.Add(newCsd);
                return newCsd;
            }
            
            public int GetStartedPlaysForChapter(int act, int chapter)
            {
                foreach (ChapterSaveData cpData in chapterSaveData)
                {
                    if (cpData.actNumber == act && cpData.chapterNumber == chapter)
                    {
                        return cpData.timesStarted;
                    }
                }

                return 0;
            }
            
            public int GetCompletedPlaysForChapter(int act, int chapter)
            {
                foreach (ChapterSaveData cpData in chapterSaveData)
                {
                    if (cpData.actNumber == act && cpData.chapterNumber == chapter)
                    {
                        return cpData.timesCompleted;
                    }
                }

                return 0;
            }
            
            public int GetIncompletePlaysForChapter(int act, int chapter)
            {
                return GetStartedPlaysForChapter(act, chapter) - GetCompletedPlaysForChapter(act, chapter);
            }

            public int GetTotalChaptersStarted()
            {
                return chapterSaveData.Sum(cpData => cpData.timesStarted);
            }
            
            public int GetTotalChaptersCompleted()
            {
                return chapterSaveData.Sum(cpData => cpData.timesCompleted);
            }
            
            public int GetTotalChaptersIncomplete()
            {
                return GetTotalChaptersStarted() - GetTotalChaptersCompleted();
            }

            public void GenerateNewPlayerUUID()
            {
                playerUUID = Guid.NewGuid().ToString();
            }

            public string GetUUID()
            {
                if (playerUUID == null)
                {
                    GenerateNewPlayerUUID();
                }
                return playerUUID;
            }
        }

        //The currently loaded user data.
        [SerializeField] private UserData userData;
        private const string SaveFileName = "SettingTheStageUserSave";
        
        public UserData GetUserData()
        {
            LoadUserData();
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
                    SaveUserData(userData);
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
                userData.GenerateNewPlayerUUID();
                userData.chapterSaveData.Clear();
                userData.totalStarsEarned = 0.0f;
                userData.highestCompletedAct = -1;
                userData.narrativesViewed.Clear();
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

        public void ChapterStarted(int actNumber, int chapterNumber)
        {
            if (userData == null)
            {
                LoadUserData();
                
                //if user data is still null, create a new user data.
                if (userData == null)
                {
                    userData = new UserData();
                    StSDebug.LogWarning($"Upon chapter beginning, no save data was found. Creating a new save.");
                }
            }

            ChapterSaveData csd = userData.GetChapterSaveData(actNumber, chapterNumber, out int index);
            csd.timesStarted += 1;
            
            // Update the current CSD in position
            userData.chapterSaveData[index] = csd;
            
            SaveUserData(userData);
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

            ChapterSaveData csd = userData.GetChapterSaveData(actNumber, chapterNumber, out int index);
            csd.timesCompleted += 1;

            // If the player has earned more stars, overwrite the save data. This includes never playing the chapter prev.
            if (starsEarned > csd.starsEarned)
            {
                csd.starsEarned = starsEarned;
                userData.totalStarsEarned += starsEarned - csd.starsEarned;
            }

            // Update the current CSD in position
            userData.chapterSaveData[index] = csd;

            // Sort by Act > Chapter > Stars
            userData.chapterSaveData.Sort();
            SaveUserData(userData);
        }

        public void ActComplete(int actNumber)
        {
            if (actNumber > userData.highestCompletedAct)
            {
                userData.highestCompletedAct = actNumber;
                SaveUserData(userData);
            }
        }

        public void SetCutsceneWatched(string cutsceneID)
        {
            userData.narrativesViewed.Add(cutsceneID);
            SaveUserData(userData);
        }

        public bool HasSeenCutscene(string cutsceneID)
        {
            return userData.narrativesViewed.Contains(cutsceneID);
        }

        public int GetTotalLevelsPlayed()
        {
            // TODO
            return userData.GetTotalChaptersCompleted();
        }

        public float GetTotalUserStars()
        {
            // TODO
            return -12.34f;
        }
        
        public int GetCountOfChapterCompletion(int act, int chapter)
        {
            return userData.GetCompletedPlaysForChapter(act, chapter);
        }
    }
}





