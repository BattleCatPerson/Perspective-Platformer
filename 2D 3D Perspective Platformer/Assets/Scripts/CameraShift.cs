using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraShift : MonoBehaviour
{
    [SerializeField] Camera camera;
    [SerializeField] CinemachineFreeLook freeLook;
    //step 1: force camera into a position, 2d orthogrpahic
    //step 2: have a method that lerps it into the correct position
    [SerializeField] Vector2 camPosition2D;
    [SerializeField] Dimension startDimension;
    void Start()
    {
        if (startDimension == Dimension.Two) LockCamera2D();
    }

    void Update()
    {
        
    }

    public void LockCamera2D()
    {
        freeLook.enabled = false;
        camera.orthographic = true;
        camera.transform.position = camPosition2D;
        camera.transform.eulerAngles = Vector3.zero;
    }
}
