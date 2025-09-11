using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audio
{
    [Serializable]
    public struct SandboxTrackProficiencyData
    {
        public InstrumentProficiency proficiency;
        public AudioClip clip;
    }
    
    [Serializable]
    public struct InstrumentTrackData
    {
        public string trackName;
        public string visibleTrackName;
        public List<SandboxTrackProficiencyData> instrumentTrackProficiencyData;
    }
    
    [Serializable]
    public struct InstrumentByTrackData
    {
        public Instrument instrumentObject;
        public string instrumentName;
        public List<InstrumentTrackData> instrumentTrackProficiencyData;
    }

    [Serializable] [CreateAssetMenu]
    public class SandboxInstrumentAudio : ScriptableObject
    {
        public List<InstrumentByTrackData> instrumentTrackData;

        public List<string> GetAllTracksByInstrument(string instrumentID)
        {
            var instrumentData = new InstrumentByTrackData();
            foreach (var instrument in instrumentTrackData.Where(instrument => instrument.instrumentObject.GetInstrumentID() == instrumentID))
            {
                instrumentData = instrument;
                break;
            }

            return instrumentData.instrumentTrackProficiencyData.Select(track => track.visibleTrackName).ToList();
        }
    }
}