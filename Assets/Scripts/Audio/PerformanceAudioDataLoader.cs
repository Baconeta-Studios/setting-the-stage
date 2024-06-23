using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Audio
{
    public class PerformanceAudioDataLoader
    {
        public PerformanceAudioData AllPerformanceData { get; private set; }
        private GameObject _dataObject;
        private readonly AsyncOperationHandle<GameObject> _handle;

        public PerformanceAudioDataLoader()
        {
            _handle = Addressables.InstantiateAsync(PerformanceAudioDataManager.AddressablePathForAudio);
            _handle.Completed += OnLoadData;
        }

        public void UnloadFromMemory()
        {
            Addressables.Release(_handle);
            if (_dataObject != null)
            {
                Addressables.ReleaseInstance(_dataObject);
            }
        }

        private void OnLoadData(AsyncOperationHandle<GameObject> data)
        {
            if (data.Status == AsyncOperationStatus.Succeeded)
            {
                _dataObject = data.Result;
                AllPerformanceData = _dataObject.GetComponent<PerformanceAudioData>();
                StSDebug.Log("Addressable object loaded - PerformanceAudioData");
            }
            else
            {
                StSDebug.LogError("Addressable object failed to load - PerformanceAudioData");
            }
        }
    }
}