using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraShift : MonoBehaviour
{
    [SerializeField] CinemachineFreeLook cam;
    //step 1: force camera into a position, 2d orthogrpahic
    //step 2: have a method that lerps it into the correct position
    [SerializeField] Vector2 camPosition2D;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
