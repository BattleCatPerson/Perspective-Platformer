using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class Launcher : MonoBehaviour
{
    public static Launcher currentLauncher;

    [SerializeField] float launchVelocity;
    [SerializeField] float rotateSpeed;
    [SerializeField] Vector3 launchDirection;
    [SerializeField] PlayerMovement player;
    [SerializeField] bool launching;
    [SerializeField] CinemachineVirtualCamera vcam; 
    void Start()
    {
        vcam.Priority = 0;
    }

    void FixedUpdate()
    {
        if (launching)
        {
            launchDirection = player.transform.forward;
            player.transform.Rotate(transform.up * rotateSpeed * Time.fixedDeltaTime);
        }
        //rotate the launchDirection transform
        //on input, launch in that direction
        //renable movement after hitting a wall/after a certain amount of time?
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.TryGetComponent<PlayerMovement>(out PlayerMovement p))
        {
            player = p;
            p.DisableMovement(false);
            p.transform.eulerAngles = Vector3.zero;
            other.transform.parent.position = transform.position;
            launching = true;
            currentLauncher = this;
            vcam.Priority = 12; // do something with the other active camera in order to make it more smooth
        }
    }

    public void Launch()
    {
        currentLauncher = null;
        launching = false;
        player.Launch(launchDirection, launchVelocity);
        vcam.Priority = 0;
    }
}
