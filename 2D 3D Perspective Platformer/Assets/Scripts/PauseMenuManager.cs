using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenuManager : MonoBehaviour
{
    public PauseMenu pauseMenu;

    [SerializeField] GameObject localCanvas;
    [SerializeField] GameObject selectedButton;

    private void Awake()
    {
        localCanvas.SetActive(false);
    }
    void Start()
    {
        pauseMenu = PauseMenu.instance;
        pauseMenu.canvas = localCanvas;

        SetPauseAsFirstSelected.instance.Select(selectedButton);
    }

    public void Resume() => pauseMenu.Resume();
    public void Restart() => pauseMenu.Restart();
    public void QuitLevel()
    {
        SetPauseAsFirstSelected.instance.Select(null);
        pauseMenu.QuitLevel();
    }

}
