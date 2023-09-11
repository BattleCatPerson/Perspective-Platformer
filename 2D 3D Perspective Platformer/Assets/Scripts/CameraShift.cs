using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraShift : MonoBehaviour
{
    [SerializeField] Camera camera;
    [SerializeField] Camera secondCamera;
    [SerializeField] CinemachineFreeLook freeLook;
    //step 1: force camera into a position, 2d orthogrpahic
    //step 2: have a method that lerps it into the correct position
    [SerializeField] Vector3 camPosition2D;
    [SerializeField] Dimension startDimension;

    [SerializeField] float transitionTime;
    [SerializeField] float currentTime;
    [SerializeField] bool transitioning;
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 startEulerAngles;

    [SerializeField] float xSpeed;
    [SerializeField] float ySpeed;
    void Start()
    {
        xSpeed = freeLook.m_XAxis.m_MaxSpeed;
        ySpeed = freeLook.m_YAxis.m_MaxSpeed;

        if (startDimension == Dimension.Two) LockCamera2D();
    }

    void FixedUpdate()
    {
        if (transitioning)
        {
            if (currentTime < transitionTime)
            {
                camera.transform.position = Vector3.Lerp(startPosition, secondCamera.transform.position, currentTime / transitionTime);
                camera.transform.eulerAngles = Vector3.Lerp(startEulerAngles, secondCamera.transform.eulerAngles, currentTime / transitionTime);
                currentTime += Time.fixedDeltaTime;
                currentTime = Mathf.Clamp(currentTime, 0, transitionTime);
            }
            else
            {
                freeLook.enabled = true;
                transitioning = false;
                camera.enabled = false;

                freeLook.m_XAxis.m_MaxSpeed = xSpeed;
                freeLook.m_YAxis.m_MaxSpeed = ySpeed;
            }
        }
    }

    public void LockCamera2D()
    {
        freeLook.m_YAxis.m_MaxSpeed = 0f;
        freeLook.m_XAxis.m_MaxSpeed = 0f;

        camera.orthographic = true;
        camera.transform.position = camPosition2D;
        camera.transform.eulerAngles = Vector3.zero;
    }

    public void LockCamera3D()
    {
        startPosition = camera.transform.position;
        startEulerAngles = camera.transform.eulerAngles;
        transitioning = true;
        camera.orthographic = false;
        freeLook.enabled = false;
    }
}
