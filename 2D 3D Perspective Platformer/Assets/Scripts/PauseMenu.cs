using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;

    public string sceneToQuit;
    
    public string mapPauseScene;
    public string levelPauseScene;
    private string currentScene;

    public static bool paused;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        sceneToQuit = GameManager.currentGameManager.sceneToQuit;
        paused = false;

        currentScene = GameManager.onMap ? mapPauseScene : levelPauseScene;
    }
    void OnPause()
    {
        if (!paused)
        {
            PauseGame();
        }
        else if (paused)
        {
            Resume();
        }
    }

    public void QuitLevel()
    {
        paused = false;
        GameManager.LoadScene(sceneToQuit);
        Time.timeScale = 1f;
    }
    public void PauseGame()
    {
        Time.timeScale = 0f;
        GameManager.LoadSceneAdditive(currentScene);
        paused = true;
    }
    public void Resume()
    {
        Time.timeScale = 1f;
        GameManager.UnloadScene(currentScene);
        paused = false;
    }
    public void Restart()
    {
        Time.timeScale = 1f;
        GameManager.UnloadScene(currentScene);
        GameManager.LoadScene();
    }

    public void Unload()
    {

    }
}
