using Utils;

namespace Audio
{
    public class PerformanceAudioDataManager : Singleton<PerformanceAudioDataManager>
    {
        public const string AddressablePathForAudio = "AllPerformanceData";

        // This class instantiates a loader and gets the data from there
        private PerformanceAudioDataLoader _dataLoader;
        private PerformanceAudioData _performanceAudioData;

        protected override void Awake()
        {
            base.Awake();
            _dataLoader = new PerformanceAudioDataLoader();
        }
    }
}