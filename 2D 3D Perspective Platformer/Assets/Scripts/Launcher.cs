using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    public static Launcher currentLauncher;

    [SerializeField] float launchVelocity;
    [SerializeField] float rotateSpeed;
    [SerializeField] Vector3 launchDirection;
    [SerializeField] PlayerMovement player;
    [SerializeField] bool launching;
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        launchDirection = transform.forward;
        if (launching)
        {
            transform.Rotate(transform.up * rotateSpeed * Time.fixedDeltaTime);
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
            other.transform.parent.position = transform.position;
            launching = true;
            currentLauncher = this;
        }
    }

    public void Launch()
    {
        currentLauncher = null;
        launching = false;
        player.Launch(launchDirection, launchVelocity);
        transform.eulerAngles = Vector3.zero;
    }
}
