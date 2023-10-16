using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceDirection : MonoBehaviour
{
    [SerializeField] Vector3 initialEulerAngles;

    [SerializeField] Vector3 finalEulerAngles;

    [SerializeField] PlayerMovement player;
    void Start()
    {
        player = GetComponentInParent<PlayerMovement>();
        GameManager.onShift += SetFinal;
        if (player.Dimension == Dimension.Two) SetIntial();
        else SetFinal();
    }
    public void SetIntial()
    {
        transform.localEulerAngles = initialEulerAngles;
    }

    public void SetFinal()
    {
        transform.localEulerAngles = finalEulerAngles;
    }

    public void SetDegree(Vector3 angle)
    {
        transform.localEulerAngles = angle;
    }
}
