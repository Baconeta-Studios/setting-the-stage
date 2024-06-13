using UnityEngine;
using UnityEngine.UI;

namespace GameStructure.Narrative
{
    public class NarrativePanelObject : MonoBehaviour
    {
        [SerializeField] private Image narrativePanelImage;

        public Image NarrativePanelImage => narrativePanelImage;

        public void SetActive(bool enable) => gameObject.SetActive(enable);
    }
}