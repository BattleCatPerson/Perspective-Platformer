using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompletionPoint : MonoBehaviour
{
    [SerializeField] string mapName;
    private void OnTriggerEnter(Collider other)
    {
        GameManager.onVictory?.Invoke();
        GameManager.LoadScene(mapName);

        string name = GameManager.currentGameManager.levelName;
        List<string> names = GameManager.completedLevelNames;
        if (!names.Contains(name))
        {
            GameManager.completedLevelNames.Add(GameManager.currentGameManager.levelName);
            GameManager.levelsCompleted++;
        }
    }
}
