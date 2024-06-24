using System;
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

        public void Init(ChapterTrackData trackData, Action bindToClose=null)
        {
            if (bindToClose != null && closePopupButton != null)
            {
                closePopupButton.onClick.AddListener(bindToClose.Invoke);
            }

            chapterTitle.text = trackData.chapterTitle;
            composerName.text = trackData.composerName;
            chapterInfo.text = trackData.chapterInfo;
            
            gameObject.SetActive(true);
        }

        public void ClosePopup()
        {
            gameObject.SetActive(false);
            closePopupButton.onClick.RemoveAllListeners();
        }
    }
}