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

    public string levelName;
    public string mapSceneName;
    public string sceneToQuit;
    private void Awake()
    {
        onShift = null;
        onVictory = null;
        currentGameManager = this;

        if (completedLevelNames == null) completedLevelNames = new();
    }

    public static void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public static void Quit()
    {
        Application.Quit();
    }
}
