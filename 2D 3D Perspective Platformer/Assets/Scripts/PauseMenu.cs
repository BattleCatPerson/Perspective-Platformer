using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public string sceneToQuit;
    public static bool paused;
    public GameObject canvas;
    public GameObject firstSelected;

    private void Start()
    {
        sceneToQuit = GameManager.currentGameManager.sceneToQuit;
        canvas.SetActive(false);
        paused = false;
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
        canvas.SetActive(true);
        paused = true;
    }
    public void Resume()
    {
        Time.timeScale = 1f;
        canvas.SetActive(false);
        paused = false;
    }
}
