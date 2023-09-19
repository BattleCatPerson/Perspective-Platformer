using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField] float launchVelocity;
    [SerializeField] Transform launchDirection;
    [SerializeField] PlayerMovement player;
    [SerializeField] bool launching;
    void Start()
    {
        
    }

    void Update()
    {
        //rotate the launchDirection transform
        //on input, launch in that direction
        //renable movement after hitting a wall/after a certain amount of time?
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerMovement>(out PlayerMovement p))
        {
            other.transform.position = transform.position;
            player = p;
            launching = true;
        }
    }
}
