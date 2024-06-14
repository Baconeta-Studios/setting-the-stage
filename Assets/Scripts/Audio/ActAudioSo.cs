using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    /// <summary>
    /// Represents all the audio data for a single Act
    /// </summary>
    public class ActAudioSo : ScriptableObject
    {
        private List<ChapterAudioSo> _chapterAudioSo;
    }
}