using UnityEngine;
using UnityEngine.UI;

namespace Settings
{
    public class SettingsPopup : MonoBehaviour
    {
        [SerializeField] private Button exitLevelButton;
        [SerializeField] private ChapterUI chapterUI;

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
    }
}