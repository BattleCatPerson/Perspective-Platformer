using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompletionPoint : MonoBehaviour
{
    [SerializeField] Collider pointCollider;
    [SerializeField] MeshRenderer pointMesh;
    [SerializeField] string mapName;
    void Start()
    {
        GameManager.onShift += EnablePoint;
        pointCollider.enabled = false;
        pointMesh.enabled = false;
    }
    public void EnablePoint()
    {
        pointCollider.enabled = true;
        pointMesh.enabled = true;
    }

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
