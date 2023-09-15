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

    [Header("Quick Rotation")]
    [SerializeField] float quickRotateAmount;
    [SerializeField] float quickRotateTime;
    [SerializeField] float currentQuickRotateTime;
    [SerializeField] bool quickRotating;
    [SerializeField] float originalRotation;
    [SerializeField] float rotateDirection;
    void Start()
    {
        xSpeed = freeLook.m_XAxis.m_MaxSpeed;
        ySpeed = freeLook.m_YAxis.m_MaxSpeed;

        if (startDimension == Dimension.Two) LockCamera2D();
        else LockCamera3D();
    }

    private void FixedUpdate()
    {
        if (quickRotating)
        {
            if (currentQuickRotateTime <= quickRotateTime)
            {
                freeLook.m_XAxis.Value = Mathf.Lerp(originalRotation, originalRotation + rotateDirection * quickRotateAmount, currentQuickRotateTime / quickRotateTime);
                currentQuickRotateTime += Time.fixedDeltaTime;
            }
            else
            {
                freeLook.m_XAxis.Value = originalRotation + rotateDirection * quickRotateAmount;
                quickRotating = false;
                SetSpeeds(true);
            }
        }
    }
    public void LockCamera2D()
    {
        SetSpeeds(false);

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
        SetSpeeds(true);
    }

    public void QuickRotate(float direction)
    {
        if (quickRotating) return;
        if (Mathf.Abs(direction) == 1)
        {
            rotateDirection = direction;
            originalRotation = freeLook.m_XAxis.Value;
            quickRotating = true;
            currentQuickRotateTime = 0f;

            SetSpeeds(false);
        }
    }

    public void SetSpeeds(bool enabled)
    {
        float x = enabled ? xSpeed : 0;
        float y = enabled ? ySpeed : 0;

        freeLook.m_XAxis.m_MaxSpeed = x;
        freeLook.m_YAxis.m_MaxSpeed = y;
    }

    public void LockCamera(Dimension d)
    {
        if (d == Dimension.Two) LockCamera2D();
        else LockCamera3D();
    }
}
