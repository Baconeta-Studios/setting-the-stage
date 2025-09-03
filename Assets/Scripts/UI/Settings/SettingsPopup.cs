using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Settings
{
    public class SettingsPopup : Popup
    {
        [SerializeField] private Button exitLevelButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private ChapterUI chapterUI;

        protected override void OnEnable()
        {
            base.OnEnable();
            optionsButton?.gameObject.SetActive(false);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            optionsButton?.gameObject.SetActive(true);
        }

        public void PressEndPerformanceEarly()
        {
            // Could have an "are you sure" option here but for now this is good
            chapterUI?.Chapter?.EndChapterEarly();
        }
        
        // Possibly we should be binding an invoke to call here on close, to ensure pausing doesn't break any flows
        // We may want to actually pause animations, and audio. For now, this is fine
        public void CloseSettingsPopup()
        {
            gameObject.SetActive(false);
        }

        public void ResetAllSaveData()
        {
            SaveSystem.Instance.ResetUserData();
        }
    }
}