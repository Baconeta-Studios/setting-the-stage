using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

[Serializable]
public enum StSSceneID
{
    MainMenu,
    GameOptions,
    Act,
    Chapter,
}

[Serializable]
public struct SceneStruct
{
    public StSSceneID sceneID;
    public string sceneDisplayName;
    public SceneAsset sceneAsset;
    public LoadSceneMode loadSceneMode;
}

public class SceneLoader : Singleton<SceneLoader>
{
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
    
    private void LoadScene(StSSceneID sceneToLoad)
    {
        LoadScene(GetSceneStructByEnum(sceneToLoad));
    }
    
    /// <summary>
    /// Loads a scene! Nice! 
    /// </summary>
    /// <param name="sceneToLoad">The scene that you want to load</param>
    private void LoadScene(SceneStruct sceneToLoad)
    {
        if (sceneToLoad.sceneAsset is not null)
        {
            Debug.Log($"Scene Opened: {sceneToLoad.sceneDisplayName}");
            
            SceneManager.LoadScene(sceneToLoad.sceneAsset.name, sceneToLoad.loadSceneMode);
            
            openScenes.Add(sceneToLoad);
        }
        else
        {
            Debug.Log($"Attempted to load '{sceneToLoad.sceneDisplayName}' but no scene asset was found. Check the SceneLoader prefab.");
        }
    }

    public void CloseScene(string sceneToClose)
    {
        CloseScene(GetSceneStructByString(sceneToClose.Trim()));
    }

    /// <summary>
    /// Closes a scene! This will produce an AsyncOperation that will report when completed.
    /// </summary>
    /// <param name="sceneToClose">The scene you want to close</param>
    void CloseScene(SceneStruct sceneToClose)
    {
        Debug.Log($"Scene Closed: {sceneToClose.sceneDisplayName}");
        
        SceneStruct scene = scenes.Find(x => x.sceneAsset.name == sceneToClose.sceneAsset.name);
        if (openScenes.Contains(scene))
        {
            openScenes.Remove(scene);
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

    SceneStruct GetSceneStructByEnum(StSSceneID sceneToGet)
    {
        return scenes.Find(x => x.sceneID == sceneToGet);
    }
    
    SceneStruct GetSceneStructByString(string sceneToGet)
    {
        return scenes.Find(x => x.sceneDisplayName == sceneToGet);
    }
}
