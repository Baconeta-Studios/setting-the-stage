using UnityEngine;

namespace GameStructure
{
    [CreateAssetMenu]
    public class ChapterTrackData : ScriptableObject
    {
        public int act;
        public int chapter;
        public string chapterTitle;
        public string composerName;
        public string chapterInfo;
    }
}