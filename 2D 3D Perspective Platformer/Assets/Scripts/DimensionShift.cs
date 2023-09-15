using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DimensionShift : MonoBehaviour
{
    [SerializeField] Dimension targetDimension;
    [SerializeField] CameraShift cameraShift;
    [SerializeField] PlayerMovement player;
    [SerializeField] bool shifted;
    public void ChangeDimension()
    {
        Debug.Log("haha");
        player.ChangeDimension(targetDimension);
        cameraShift.LockCamera(targetDimension);
        shifted = true;

        GameManager.onShift?.Invoke();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<PlayerMovement>() && !shifted) ChangeDimension();
    }
}
