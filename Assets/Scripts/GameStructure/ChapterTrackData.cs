using System.Collections.Generic;
using System.Linq;
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

        public List<Instrument> correctInstruments;
        public List<Instrument> veryBadInstruments;

        /// <summary>
        /// Returns a score between -5 and 5 based on whether this instrument belongs in this track or not.
        /// </summary>
        /// <param name="instrument"></param>
        /// <returns></returns>
        public int GetInstrumentScore(Instrument instrument)
        {
            if (correctInstruments.Any(instrument.Equals))
            {
                return 5;
            }
            if (veryBadInstruments.Any(instrument.Equals))
            {
                return -5;
            }

            return 0;
        }
    }
}