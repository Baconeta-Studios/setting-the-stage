using System;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    [Serializable] [CreateAssetMenu]
    public class ActAudioSo : ScriptableObject
    {
        public int actNumber;
        public List<ChapterAudioSo> chapterAudioSo;
    }
}