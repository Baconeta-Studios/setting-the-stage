using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Audio
{
    public class PerformanceAudioDataLoader
    {
        public PerformanceAudioData AllPerformanceData { get; private set; }

        public PerformanceAudioDataLoader()
        {
            Addressables.LoadAssetAsync<GameObject>(PerformanceAudioDataManager.AddressablePathForAudio).Completed += OnLoadData;
        }

        private void OnLoadData(AsyncOperationHandle<GameObject> data)
        {
            GameObject dataObject = data.Result;
            AllPerformanceData = dataObject.GetComponent<PerformanceAudioData>();
        }
    }
}