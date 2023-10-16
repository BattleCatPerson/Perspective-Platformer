using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class Launcher : MonoBehaviour
{
    public static Launcher currentLauncher;

    [Header("Launching")]
    [SerializeField] float launchVelocity;
    [SerializeField] float rotateSpeed;
    [SerializeField] Vector3 launchDirection;
    [SerializeField] Transform launcherModel;
    [SerializeField] bool verticalOrHorizontal;
    [Header("Player")]
    [SerializeField] PlayerMovement player;
    [SerializeField] Transform model;
    [SerializeField] bool launching;
    [SerializeField] CinemachineVirtualCamera vcam;
    [Header("Indication")]
    [SerializeField] Transform indicatorPivot;
    [SerializeField] Transform indicator;
    void Start()
    {
        vcam.Priority = 0;
        SetIndicator2D();
        GameManager.onShift += SetIndicator3D;
    }

    void FixedUpdate()
    {
        if (launching)
        {
            indicator.gameObject.SetActive(true);
            if (verticalOrHorizontal)
            {
                Debug.Log(transform.forward);
                Debug.Log(launcherModel.forward);

                launchDirection = model.transform.right;
                model.Rotate(transform.forward * rotateSpeed * Time.fixedDeltaTime);
                indicatorPivot.transform.right = launchDirection;
            }
            else
            {
                launchDirection = model.transform.forward;
                model.Rotate(transform.up * rotateSpeed * Time.fixedDeltaTime);
                indicatorPivot.transform.forward = launchDirection;
            }

        }
        else indicator.gameObject.SetActive(false);
        //rotate the launchDirection transform
        //on input, launch in that direction
        //renable movement after hitting a wall/after a certain amount of time?
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.TryGetComponent<PlayerMovement>(out PlayerMovement p))
        {
            player = p;
            p.transform.parent = transform;
            model = p.Model;
            p.DisableMovement(false);
            p.CameraShift.FreeLook.m_XAxis.Value = 0;

            model.localEulerAngles = new(model.localEulerAngles.x, model.localEulerAngles.y, 0);
            other.transform.parent.position = transform.position;
            launching = true;
            currentLauncher = this;
            //make sure that gravit is not enabled again if you are in the launcher
            if (p.Dimension == Dimension.Three) vcam.Priority = 12;

            var obj = CameraShift.instance.currentCamera;
            if (obj.TryGetComponent<CinemachineFreeLook>(out CinemachineFreeLook c))
            {
                c.LookAt = null;
                c.Follow = null;
            }
        }
    }

    public void Launch()
    {
        player.transform.parent = null;

        currentLauncher = null;
        launching = false;
        player.Launch(launchDirection, launchVelocity);
        vcam.Priority = 0;

        var obj = CameraShift.instance.currentCamera;
        if (obj.TryGetComponent<CinemachineFreeLook>(out CinemachineFreeLook c))
        {
            c.LookAt = PlayerMovement.instance.transform;
            c.Follow = PlayerMovement.instance.transform;
        }
    }

    public void SetIndicator2D()
    {
        indicator.localEulerAngles = Vector3.zero;
        indicator.localPosition = Vector3.right;
        indicator.transform.localScale = new(1, 0.5f);
    }

    public void SetIndicator3D()
    {
        indicator.localEulerAngles = new(90, 0, 90);
        indicator.localPosition = Vector3.forward;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, launchDirection);
    }
}
