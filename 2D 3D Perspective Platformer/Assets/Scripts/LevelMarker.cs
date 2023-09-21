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

    [SerializeField] List<LevelMarker> markersInRange;

    [Header("Level Info")]
    public LevelInfo levelInfo;

    [Header("Marker Rotation")]
    [SerializeField] Transform platformTransform;
    [SerializeField] float rotationSpeed;


    [Header("Display Information")]
    public float distance;
    [SerializeField] float maxDistance; //distance player needs to be from marker to display info
    [SerializeField] bool showInfo;
    [SerializeField] GameObject canvas;
   
    void Start()
    {
        Hide();
    }

    void FixedUpdate()
    {
        markersInRange = LevelMarkerManager.markersInRange;
        platformTransform.Rotate(transform.up * rotationSpeed * Time.fixedDeltaTime);

        if (!PlayerMovement.instance) return;
        distance = Vector3.Distance(PlayerMovement.instance.transform.position, transform.position);
        if (distance <= maxDistance && !markersInRange.Contains(this)) markersInRange.Add(this);
        else if (distance > maxDistance && markersInRange.Contains(this))
        {
            markersInRange.Remove(this);
            Hide();
        }
    }

    public void Show()
    {
        canvas.SetActive(true);
    }

    public void Hide()
    {
        canvas.SetActive(false);
    }
}
