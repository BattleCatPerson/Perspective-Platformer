using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PauseMenu : MonoBehaviour
{
    public string sceneToQuit;
    public static bool paused;
    public GameObject canvas;

    private int pauseInputInOneFrame = 0;
    private void Start()
    {
        sceneToQuit = GameManager.currentGameManager.sceneToQuit;
        canvas.SetActive(false);
        paused = false;
    }
    private void Update()
    {
        //if (pauseInputInOneFrame > 0) pauseInputInOneFrame = 0;
    }
    void OnPause()
    {
        pauseInputInOneFrame++;
        if (pauseInputInOneFrame > 1) return;
        Debug.Log(pauseInputInOneFrame); 
        if (!paused)
        {
            PauseGame();
        }
        else Resume();
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
        Debug.Log("HEE");
    }
    public void Resume()
    {
        Time.timeScale = 1f;
        canvas.SetActive(false);
        paused = false;
        Debug.Log("HEE2");
    }
}
