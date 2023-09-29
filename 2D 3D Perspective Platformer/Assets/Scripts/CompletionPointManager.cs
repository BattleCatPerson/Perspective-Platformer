using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompletionPointManager : MonoBehaviour
{
    public GameObject completionPoint;

    private void Start()
    {
        GameManager.onShift += EnablePoint;
        completionPoint.SetActive(false);
    }

    public void EnablePoint()
    {
        completionPoint.SetActive(true);
    }
}
