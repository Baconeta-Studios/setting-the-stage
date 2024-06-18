using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Analytics
{
    public class SaveManager : EverlastingSingleton<SaveManager>
    {
        private string _level_id;
        private string _act_id;
        private Queue<int> _level_play_history;
        // Uses _level_id as the key.
        private IDictionary<int, int> _times_played_before;
        // Stored in the actual game logic.
        private int _score;
        // Uses _level_id as the key. Values are half-stars.
        private IDictionary<int, int> _level_highscores;

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

        public int GetTotalLevelsPlayed()
        {
            return _level_play_history.Count;
        }

        public int GetUniqueLevelsPlayed()
        {

        }

        public int GetTimesLevelWasPlayedBefore(int level_id)
        {
            bool exists = _times_played_before.TryGetValue(level_id, out int result);
            return exists ? result : 0;
        }

        public int GetHighscoreForLevel(int level_id)
        {   
            bool exists = _level_highscores.TryGetValue(level_id, out int result);
            return exists ? result : 0;
        }
    }
}
