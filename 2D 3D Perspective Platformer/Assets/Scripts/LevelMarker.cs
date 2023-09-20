using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelMarker : MonoBehaviour
{
    [Serializable]
    public class LevelInfo
    {
        public string name;
        public string sceneName;
    }

    [Header("Level Info")]
    [SerializeField] LevelInfo levelInfo;

    [Header("Marker Rotation")]
    [SerializeField] Transform platformTransform;
    [SerializeField] float rotationSpeed;

    
    [Header("Display Information")]
    [SerializeField] float maxDistance; //distance player needs to be from marker to display info
    [SerializeField] bool showInfo;
    [SerializeField] GameObject canvas;
    void Start()
    {

    }

    void FixedUpdate()
    {
        platformTransform.Rotate(transform.up * rotationSpeed * Time.fixedDeltaTime);

        if (!PlayerMovement.instance) return; 
        if (Vector3.Distance(PlayerMovement.instance.transform.position, transform.position) <= maxDistance) Show();
        else Hide();
    }

    void Show()
    {
        Debug.Log("SHWOING");
        canvas.SetActive(true);
    }

    void Hide()
    {
        Debug.Log("HIDING");
        canvas.SetActive(false);
    }
}
