using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class SelectStage : MonoBehaviour
{
    [SerializeField] LevelMarker currentMarker;
    void OnJump()
    {
        if (LevelMarkerManager.markersInRange.Count > 0) SceneManager.LoadScene(currentMarker.levelInfo.sceneName);
    }

    private void Update()
    {
        if (LevelMarkerManager.markersInRange.Count > 0)
        {
            int index = 0;
            float minDistance = Mathf.Infinity;
            for (int i = 0; i < LevelMarkerManager.markersInRange.Count; i++)
            {
                LevelMarker l = LevelMarkerManager.markersInRange[i];
                if (l.distance < minDistance)
                {
                    minDistance = l.distance;
                    index = i;
                }
            }

            LevelMarker newMarker = LevelMarkerManager.markersInRange[index];
            if (currentMarker && newMarker != currentMarker) currentMarker.Hide();
            newMarker.Show();
            currentMarker = newMarker;
        }
        else currentMarker = null;
    }
}
