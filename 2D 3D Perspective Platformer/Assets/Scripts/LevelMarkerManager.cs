using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMarkerManager : MonoBehaviour
{
    public static List<LevelMarker> markersInRange;
    private void Awake()
    {
        markersInRange = new();
    }
}
