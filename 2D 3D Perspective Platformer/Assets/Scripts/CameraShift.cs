using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
public class CameraShift : MonoBehaviour
{
    public static CameraShift instance;
    [SerializeField] Camera cam;
    [SerializeField] CinemachineVirtualCamera cinemachine2D;
    [SerializeField] CinemachineFreeLook freeLook;
    public CinemachineFreeLook FreeLook => freeLook;

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
    [SerializeField] bool canQuickRotate;

    public Transform currentCamera;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
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
        SetPriority(2);

        cinemachine2D.transform.position = cinemachine2D.transform.position;
        cinemachine2D.transform.eulerAngles = Vector3.zero;
        cam.orthographic = true;

        currentCamera = cinemachine2D.transform;
    }

    public void LockCamera3D()
    {
        SetPriority(3);
        cam.orthographic = false;
        StartCoroutine(DelaySpeed());
        currentCamera = freeLook.transform;
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
    public IEnumerator DelaySpeed(float t)
    {
        SetSpeeds(false);
        yield return new WaitForSeconds(t);
        SetSpeeds(true);
    }

    public void QuickRotate(float direction)
    {
        if (quickRotating || !canQuickRotate) return;
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

        canQuickRotate = enabled;
    }

    public void LockCamera(Dimension d)
    {
        if (d == Dimension.Two) LockCamera2D();
        else LockCamera3D();
    }
}
