using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMarkerManager : MonoBehaviour
{
    public static List<LevelMarker> markersInRange;
    public static Vector3 mapSpawnPosition = new Vector3();
    private void Awake()
    {
        markersInRange = new();
    }
}
