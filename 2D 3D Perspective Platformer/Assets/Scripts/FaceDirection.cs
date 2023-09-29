using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceDirection : MonoBehaviour
{
    [SerializeField] float initialZ;
    [SerializeField] Vector3 initialEulerAngles;

    [SerializeField] float finalZ;
    [SerializeField] Vector3 finalEulerAngles;
    void Start()
    {
        GameManager.onShift += SetFinal;
        SetIntial();        
    }
    public void SetIntial()
    {
        transform.localPosition = Vector3.forward * initialZ;
        transform.localEulerAngles = initialEulerAngles;
    }

    public void SetFinal()
    {
        transform.localPosition = Vector3.forward * finalZ;
        transform.localEulerAngles = finalEulerAngles;
    }
}
