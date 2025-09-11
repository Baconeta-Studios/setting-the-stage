using UnityEngine;
using Utils;

namespace Audio
{
    public class SandboxAudioDataManager : Singleton<SandboxAudioDataManager>
    {
        // This class instantiates a loader and gets the data from there
        private SandboxAudioDataLoader _dataLoader;
        public static string AddressablePathForSandboxAudio => "SandboxPerformanceData";

        private void OnEnable()
        {
            LoadAudioData();
        }

        private void OnDisable()
        {
            UnloadAudioData();
        }

        private void LoadAudioData()
        {
            _dataLoader = new SandboxAudioDataLoader();
        }

        private void UnloadAudioData()
        {
            _dataLoader?.UnloadFromMemory();
        }
        
        public AudioClip GetAudioTrack(Instrument instrument, string trackName, InstrumentProficiency proficiency)
        {
            // Based on the given values we want to give back the caller the correct audio track
            if (_dataLoader?.SandboxAudioData is null)
            {
                StSDebug.LogError("Somehow we don't actually have any data ready yet");
                return null;
            }

            var audioData = _dataLoader.SandboxAudioData;

            // For sandbox mode, we simply go for any instrument, from anywhere in the game (todo, for now)
            StSDebug.Log($"Searching for {instrument} in sandbox audio data");
            if (audioData.TryGetAudioByTrackName(trackName, instrument.GetInstrumentID(), proficiency, out AudioClip clip))
            {
                return clip;
            }

            // Finally, we somehow have an instrument for which we do not have a matching set of data, so we throw an error
            StSDebug.LogError($"We don't have a single track for {instrument} matching proficiency level {proficiency}.");
            return null;
        }
    }
}