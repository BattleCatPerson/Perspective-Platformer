using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager currentGameManager;
    public static List<string> completedLevelNames;
    public static int levelsCompleted = 0;

    public bool victory;
    public bool shifted;

    public static Action onShift;
    public static Action onVictory;
    public static bool onMap;

    public string levelName;
    public string mapSceneName;
    public string sceneToQuit;

    
    private void Awake()
    {
        onShift = null;
        onVictory = null;
        currentGameManager = this;
        onMap = SceneManager.GetActiveScene().name == mapSceneName;

        if (completedLevelNames == null) completedLevelNames = new();
    }
    public static void LoadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public static void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
    public static void UnloadScene(string scene)
    {
        SceneManager.UnloadSceneAsync(scene);
    }
    public static void LoadSceneAdditive(string scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
    }
    public static void Quit()
    {
        Application.Quit();
    }

    
}
