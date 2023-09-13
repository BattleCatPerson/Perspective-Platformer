using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
public class CameraShift : MonoBehaviour
{
    [SerializeField] Camera camera;
    [SerializeField] CinemachineVirtualCamera cinemachine2D;
    [SerializeField] CinemachineFreeLook freeLook;

    [SerializeField] Vector3 camPosition2D;
    [SerializeField] Dimension startDimension;

    [SerializeField] float xSpeed;
    [SerializeField] float ySpeed;

    [SerializeField] float setSpeedDelay;

    [SerializeField] float quickRotateAmount;
    [SerializeField] float quickRotateTime;
    [SerializeField] bool quickRotating;
    [SerializeField] float originalRotation;
    void Start()
    {
        xSpeed = freeLook.m_XAxis.m_MaxSpeed;
        ySpeed = freeLook.m_YAxis.m_MaxSpeed;

        if (startDimension == Dimension.Two) LockCamera2D();
        else LockCamera3D();
    }

    public void LockCamera2D()
    {
        freeLook.m_YAxis.m_MaxSpeed = 0f;
        freeLook.m_XAxis.m_MaxSpeed = 0f;

        camera.orthographic = true;
        cinemachine2D.transform.position = camPosition2D;
        cinemachine2D.transform.eulerAngles = Vector3.zero;

        SetPriority(2);
    }

    public void LockCamera3D()
    {
        SetPriority(3);
        camera.orthographic = false;
        StartCoroutine(DelaySpeed());
    }

    public void SetPriority(int dimension)
    {
        if (dimension == 2)
        {
            cinemachine2D.Priority = 11;
            freeLook.Priority = 10;
        }
        if (dimension == 3)
        {
            cinemachine2D.Priority = 10;
            freeLook.Priority = 11;
        }
    }

    public IEnumerator DelaySpeed()
    {
        yield return new WaitForSeconds(setSpeedDelay);
        freeLook.m_XAxis.m_MaxSpeed = xSpeed;
        freeLook.m_YAxis.m_MaxSpeed = ySpeed;
    }

    void OnQuickRotateCamera(InputValue value)
    {
        float direction = value.Get<float>();
        if (Mathf.Abs(direction) == 1) freeLook.m_XAxis.Value += direction * quickRotateAmount;
    }
}
