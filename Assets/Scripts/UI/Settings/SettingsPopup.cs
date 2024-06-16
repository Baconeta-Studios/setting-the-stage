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
    }
}