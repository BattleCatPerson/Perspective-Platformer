using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventSystem))]
public class SetPauseAsFirstSelected : MonoBehaviour
{
    [SerializeField] EventSystem system;
    private void Start()
    { 
        system.SetSelectedGameObject(PlayerMovement.instance.GetComponent<PauseMenu>().firstSelected);
    }
}
