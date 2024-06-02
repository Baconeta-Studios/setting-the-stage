using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

[Serializable]
public struct SceneStruct
{
    public string sceneDisplayName;
    public SceneAsset sceneAsset;
    public LoadSceneMode loadSceneMode;
}

public class SceneLoader : Singleton<SceneLoader>
{
    public event Action onSceneOpened;
    
    [Header("Scenes")] 
    [SerializeField] 
    private List<SceneStruct> scenes;
    [SerializeField]
    private List<SceneStruct> openScenes = new List<SceneStruct>();


    void OnEnable()
    {
        // Make sure that the current scene is accounted for in open scenes
        SceneStruct currentScene = scenes.Find(x => x.sceneAsset.name == SceneManager.GetActiveScene().name);
        if (!openScenes.Contains(currentScene))
        {
            openScenes.Add(currentScene);
        }
    }

    public void LoadScene(string sceneToLoad)
    {
        LoadScene(GetSceneStructByString(sceneToLoad.Trim()));
    }

    /// <summary>
    /// Loads a scene! Nice! 
    /// </summary>
    /// <param name="sceneToLoad">The scene that you want to load</param>
    public bool LoadScene(SceneStruct sceneToLoad)
    {
        if (sceneToLoad.sceneAsset is not null)
        {
            Debug.Log($"Open Scene: {sceneToLoad.sceneDisplayName}");
            
            AsyncOperation loadSceneOperation = SceneManager.LoadSceneAsync(sceneToLoad.sceneAsset.name, sceneToLoad.loadSceneMode);
            openScenes.Add(sceneToLoad);
            loadSceneOperation.completed += SceneOpened;
            return true;
        }
 
        Debug.Log($"Attempted to load '{sceneToLoad.sceneDisplayName}' but no scene asset was found.");
        return false;
    }

    void SceneOpened(AsyncOperation loadOperation)
    {
        Debug.Log($"Scene opened successfully");
        onSceneOpened?.Invoke();
    }

    public void CloseScene(string sceneToClose)
    {
        CloseScene(GetSceneStructByString(sceneToClose.Trim()));
    }

    /// <summary>
    /// Closes a scene! This will produce an AsyncOperation that will report when completed.
    /// </summary>
    /// <param name="sceneToClose">The scene you want to close</param>
    public void CloseScene(SceneStruct sceneToClose)
    {
        Debug.Log($"Close Scene: {sceneToClose.sceneDisplayName}");
        
        if (openScenes.Contains(sceneToClose))
        {
            openScenes.Remove(sceneToClose);
        }
        
        // Unloads the scene and bind to the async operation.
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(sceneToClose.sceneAsset.name);
        unloadOperation.completed += SceneClosed;
    }

    /// <summary>
    /// After a scene has successfully been unloaded, this function will fire.
    /// </summary>
    /// <param name="unloadOperation"></param>
    void SceneClosed(AsyncOperation unloadOperation)
    {
        Debug.Log($"Scene closed successfully");
        unloadOperation.completed -= SceneClosed;
    }

    SceneStruct GetSceneStructByString(string sceneToGet)
    {
        return scenes.Find(x => x.sceneDisplayName == sceneToGet);
    }

    public bool CanLoadScene(string sceneString)
    {
        return GetSceneStructByString(sceneString).sceneAsset is not null;
    }
}
