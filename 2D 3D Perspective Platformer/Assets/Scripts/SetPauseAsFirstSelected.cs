using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventSystem))]
public class SetPauseAsFirstSelected : MonoBehaviour
{
    public static SetPauseAsFirstSelected instance;
    [SerializeField] EventSystem eventSystem;
    private void Awake()
    {
        instance = this;
        eventSystem = GetComponent<EventSystem>();
    }
    public void Select(GameObject selected)
    {
        eventSystem.SetSelectedGameObject(selected);
    }
}
