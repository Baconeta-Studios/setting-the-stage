using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameStructure
{
    public class ChapterPopup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI chapterTitle;
        [SerializeField] private TextMeshProUGUI composerName;
        [SerializeField] private TextMeshProUGUI chapterInfo;
        [SerializeField] private Button closePopupButton;
    }
}