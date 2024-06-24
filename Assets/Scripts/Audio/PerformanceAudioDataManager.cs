using System.Linq;
using UnityEngine;
using Utils;

namespace Audio
{
    public class PerformanceAudioDataManager : Singleton<PerformanceAudioDataManager>
    {
        public const string AddressablePathForAudio = "AllPerformanceData";

        // This class instantiates a loader and gets the data from there
        private PerformanceAudioDataLoader _dataLoader;

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
            _dataLoader = new PerformanceAudioDataLoader();
        }

        private void UnloadAudioData()
        {
            _dataLoader?.UnloadFromMemory();
        }

        public AudioClip GetAudioTrack(int actNumber, int chapterNumber, Instrument instrument, InstrumentProficiency proficiency)
        {
            // Based on the given values we want to give back the caller the correct audio track
            if (_dataLoader is null || _dataLoader.AllPerformanceData is null)
            {
                StSDebug.LogError("Somehow we don't actually have any data ready yet");
                return null;
            }

            PerformanceAudioData audioData = _dataLoader.AllPerformanceData;

            // First we look for data in the correct act and chapter
            StSDebug.Log($"Searching for {instrument} in act {actNumber} data in chapter {chapterNumber}");
            if (TryGetClipFromActAndChapter(actNumber, chapterNumber, instrument, proficiency, audioData,
                    out AudioClip audioTrack))
            {
                return audioTrack;
            }

            // If that instrument does not have a matching track in the chapter, find one from the rest of the act
            StSDebug.Log($"Searching for {instrument} in act {actNumber} data in any chapter");
            if (TryGetTrackFromAnyChapter(actNumber, instrument, proficiency, audioData, out AudioClip audioClip))
            {
                return audioClip;
            }

            // If somehow that doesn't exist, we now reach to the rest of the acts
            StSDebug.Log($"Searching for {instrument} in any act, in any chapter");
            if (TryGetTrackFromAnyAct(instrument, proficiency, audioData, out AudioClip clip))
            {
                return clip;
            }

            // Finally, we somehow have an instrument for which we do not have a matching set of data, so we throw an error
            StSDebug.LogError($"We don't have a single track for {instrument} matching proficiency level {proficiency}.");
            return null;
        }

        private static bool TryGetTrackFromAnyAct(Instrument instrument, InstrumentProficiency proficiency, PerformanceAudioData audioData, out AudioClip audioClip)
        {
            foreach (InstrumentTrackProficiencyData instrumentData in from act in audioData.actAudioData
                     from chapter in act.chapterAudioSo
                     from instrumentData in chapter.instrumentTrackProficiencyData
                     where instrumentData.instrument.Equals(instrument) && instrumentData.proficiency == proficiency
                     select instrumentData)
            {
                StSDebug.Log($"Found track {instrumentData.clip} matching parameters in another act.");
                {
                    audioClip = instrumentData.clip;
                    return true;
                }
            }

            audioClip = null;
            return false;
        }

        private static bool TryGetTrackFromAnyChapter(int actNumber, Instrument instrument, InstrumentProficiency proficiency, PerformanceAudioData audioData, out AudioClip audioClip)
        {
            foreach (InstrumentTrackProficiencyData instrumentData in from act in audioData.actAudioData
                     where act.actNumber == actNumber
                     from chapter in act.chapterAudioSo
                     from instrumentData in chapter.instrumentTrackProficiencyData
                     where instrumentData.instrument.Equals(instrument) && instrumentData.proficiency == proficiency
                     select instrumentData)
            {
                StSDebug.Log($"Found track {instrumentData.clip} matching parameters in another chapter.");
                {
                    audioClip = instrumentData.clip;
                    return true;
                }
            }

            audioClip = null;
            return false;
        }

        private static bool TryGetClipFromActAndChapter(int actNumber, int chapterNumber, Instrument instrument, InstrumentProficiency proficiency, PerformanceAudioData audioData, out AudioClip audioClip)
        {
            foreach (InstrumentTrackProficiencyData instrumentData in from act in audioData.actAudioData
                     where act.actNumber == actNumber
                     from chapter in act.chapterAudioSo
                     where chapter.chapterNumber == chapterNumber
                     from instrumentData in chapter.instrumentTrackProficiencyData
                     where instrumentData.instrument.Equals(instrument) && instrumentData.proficiency == proficiency
                     select instrumentData)
            {
                StSDebug.Log($"Found track {instrumentData.clip} matching parameters.");
                {
                    audioClip = instrumentData.clip;
                    return true;
                }
            }

            audioClip = null;
            return false;
        }
    }
}