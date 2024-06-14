using System;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    [Serializable] [CreateAssetMenu]
    public class ActAudioSo : ScriptableObject
    {
        [SerializeField] private List<ChapterAudioSo> chapterAudioSo;
    }
}