using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Achievements
{
    [Serializable]
    public enum AchievementType
    {
        Bool,
        Int,
        Float
    }

    [Serializable]
    public class AchievementSettingsBase
    {
        public AchievementType achievementType;
        public string triggerValue;
    }

    [Serializable]
    public class Achievement
    {
        public string achievementName;

        [Tooltip("Name used to reference the prefs values")]
        public string achievementUserPrefsCodeName;

        public string subMessage = "???";

        [HideInInspector] public bool completed;

        public AchievementSettingsBase achievementSettings;

        private float _internalRequiredValue;

        public void SetInternalTriggerValue()
        {
            switch (achievementSettings.achievementType)
            {
                case AchievementType.Bool:
                    _internalRequiredValue = 1.0f;
                    break;
                case AchievementType.Int:
                    _internalRequiredValue = int.Parse(achievementSettings.triggerValue);
                    break;
                case AchievementType.Float:
                    _internalRequiredValue = float.Parse(achievementSettings.triggerValue);
                    break;
                default:
                    Debug.Log(
                        $"Achievement {achievementName} trigger value set incorrectly for type {achievementSettings.achievementType}");
                    break;
            }
        }

        public bool CheckCompletion()
        {
            return achievementSettings.achievementType switch
            {
                AchievementType.Bool => AchievementController.Instance.GetBool(achievementUserPrefsCodeName),
                AchievementType.Int => AchievementController.Instance.GetInt(achievementUserPrefsCodeName) >=
                                       (int) _internalRequiredValue,
                AchievementType.Float => AchievementController.Instance.GetFloat(achievementUserPrefsCodeName) >=
                                         _internalRequiredValue,
                _ => false
            };
        }

        public void PurgeFromCache()
        {
            this.completed = false;
            PlayerPrefs.DeleteKey(achievementUserPrefsCodeName);
        }
    }

    public class AchievementController : EverlastingSingleton<AchievementController>
    {
        [SerializeField] private List<AchievementData> allAchievements;

        private List<Achievement> _achievements = new();

        public List<Achievement> Achievements => _achievements;

        public void Start()
        {
            // Marks off any tasks that should be already be marked off as being completed prior to this session.
            CheckCompletedAchievements();

            if (UnityHelper.IsNotInUnityEditor)
            {
                SetupAndVerifyAchievements();
            }
        }

        public List<Achievement> GetCompletedAchievements()
        {
            return Achievements.Where(achievement => achievement.completed).ToList();
        }

        public List<Achievement> CheckCompletedAchievements()
        {
            var newlyCompleted = new List<Achievement>();

            foreach (Achievement ach in Achievements)
            {
                if (ach.completed) continue;

                // DO NOT APPROVE ME
                // Why is this check made? Doesn't needing this explicit check make '.completed' a useless field?
                if (ach.CheckCompletion())
                {
                    newlyCompleted.Add(ach);
                    ach.completed = true;
                    Achievements[i] = ach;
                }
            }

            return newlyCompleted;
        }

        public void ResetAllAchievements()
        {
            // Instruct each achievement object to wipe itself from PlayerPrefs.
            _achievements.ForEach(a => a.PurgeFromCache());

            // Update completed achievement data.
            CheckCompletedAchievements();
        }

        private void SetupAndVerifyAchievements()
        {
            foreach (AchievementData oldAch in allAchievements)
            {
                if (oldAch == null) continue;

                Achievement newAch = new()
                {
                    achievementSettings = oldAch.achievementSettings,
                    achievementName = oldAch.achievementName,
                    achievementUserPrefsCodeName = oldAch.achievementUserPrefsCodeName,
                    subMessage = oldAch.subMessage
                };
                newAch.SetInternalTriggerValue();
                _achievements.Add(newAch);
            }
        }

        private void OnValidate()
        {
            if (UnityHelper.IsInUnityEditor)
            {
                SetupAndVerifyAchievements();
            }
        }

        public void SetInt(string keyName, int value)
        {
            PlayerPrefs.SetInt(keyName, value);
        }

        public int GetInt(string keyName)
        {
            return PlayerPrefs.GetInt(keyName, 0);
        }

        public void AddInt(string keyName, int value)
        {
            PlayerPrefs.SetInt(keyName, GetInt(keyName) + value);
        }

        public float GetFloat(string keyName)
        {
            return PlayerPrefs.GetFloat(keyName, 0.0f);
        }

        public void AddFloat(string keyName, int value)
        {
            PlayerPrefs.SetFloat(keyName, GetFloat(keyName) + value);
        }

        public void SetBool(string boolName, bool value)
        {
            PlayerPrefs.SetInt(boolName, value ? 1 : 0);
        }

        public bool GetBool(string boolName)
        {
            return PlayerPrefs.GetInt(boolName) == 1;
        }
    }
}