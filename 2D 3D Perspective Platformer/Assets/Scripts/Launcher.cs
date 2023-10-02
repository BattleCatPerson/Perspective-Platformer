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
    }

    void FixedUpdate()
    {
        if (launching)
        {
            indicator.gameObject.SetActive(true);
            if (PlayerMovement.instance.Dimension == Dimension.Two)
            {
                launchDirection = model.transform.right;
                model.Rotate(transform.forward * rotateSpeed * Time.fixedDeltaTime);

                indicator.localEulerAngles = Vector3.zero;
                indicator.localPosition = Vector3.right;
                indicatorPivot.transform.right = launchDirection;
            }
            else
            {
                launchDirection = model.transform.forward;
                model.Rotate(transform.up * rotateSpeed * Time.fixedDeltaTime);

                indicator.localScale = new(1, 0.5f);
                indicator.localPosition = Vector3.forward;
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
            model = p.Model;
            p.DisableMovement(false);
            model.localEulerAngles = new(model.localEulerAngles.x, model.localEulerAngles.y, 0);
            other.transform.parent.position = transform.position;
            launching = true;
            currentLauncher = this;
            
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
}
