using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Audio
{
    public class PerformanceAudioDataLoader
    {
        public PerformanceAudioData AllPerformanceData { get; private set; }
        private GameObject _dataObject;

        public PerformanceAudioDataLoader()
        {
            Addressables.LoadAssetAsync<GameObject>(PerformanceAudioDataManager.AddressablePathForAudio).Completed += OnLoadData;
        }

        public void UnloadFromMemory()
        {
            Addressables.ReleaseInstance(_dataObject);
        }

        private void OnLoadData(AsyncOperationHandle<GameObject> data)
        {
            if (data.Status == AsyncOperationStatus.Succeeded)
            {
                _dataObject = data.Result;
                AllPerformanceData = _dataObject.GetComponent<PerformanceAudioData>();
            }
            else
            {
                StSDebug.LogError("Addressable object failed to load - PerformanceAudioData");
            }
        }
    }
}