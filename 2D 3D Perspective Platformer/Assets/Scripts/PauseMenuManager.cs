using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenuManager : MonoBehaviour
{
    public PauseMenu pauseMenu;
    [SerializeField] GameObject selectedButton;

    void Start()
    {
        pauseMenu = PauseMenu.instance;
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
