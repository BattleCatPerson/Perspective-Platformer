using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMarkerManager : MonoBehaviour
{
    public static List<LevelMarker> markersInRange;
    public static Vector3 mapSpawnPosition = new Vector3();

    [SerializeField] List<GameObject> markers;
    private void Awake()
    {
        markersInRange = new();
        for (int i = 0; i <= GameManager.levelsCompleted; i++)
        {
            markers[i].SetActive(true);
        }
    }
}
