using System;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    [Serializable]
    public struct InstrumentTrackProficiencyData
    {
        public Instrument instrument;
        public InstrumentProficiency proficiency;
        public AudioClip clip;
    }

    [Serializable] [CreateAssetMenu]
    public class ChapterAudioSo : ScriptableObject
    {
        public int chapterNumber;
        public string trackName;
        public List<InstrumentTrackProficiencyData> instrumentTrackProficiencyData;
    }
}