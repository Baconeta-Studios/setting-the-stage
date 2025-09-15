using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Audio
{
    public class SandboxAudioDataLoader
    {
        public SandboxInstrumentAudio SandboxAudioData { get; private set; }
        private ScriptableObject _dataObject;
        private readonly AsyncOperationHandle<ScriptableObject> _handle;

        public SandboxAudioDataLoader()
        {
            _handle = Addressables.LoadAssetAsync<ScriptableObject>(SandboxAudioDataManager.AddressablePathForSandboxAudio);
            _handle.Completed += OnLoadData;
        }

        public void UnloadFromMemory()
        {
            Addressables.Release(_handle);
            //TODO look at this spot if we have performance issues from addressables
        }

        private void OnLoadData(AsyncOperationHandle<ScriptableObject> data)
        {
            if (data.Status == AsyncOperationStatus.Succeeded)
            {
                _dataObject = data.Result;
                SandboxAudioData = _dataObject as SandboxInstrumentAudio;
                StSDebug.Log("Addressable object loaded - PerformanceAudioData");
            }
            else
            {
                StSDebug.LogError("Addressable object failed to load - PerformanceAudioData");
            }
        }
    }
}