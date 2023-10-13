using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchStopper : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == PlayerMovement.instance.Model) PlayerMovement.instance.StopLaunch();
        Debug.Log(other.transform);
    }
}
