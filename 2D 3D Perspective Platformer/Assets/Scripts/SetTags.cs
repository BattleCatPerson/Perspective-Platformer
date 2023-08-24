using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public class SetTags : MonoBehaviour
{
    public List<GameObject> floors;
    public string tagName;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Assign()
    {
        for (int i = 0; i < transform.childCount; i++) transform.GetChild(i).tag = tagName;
    }
}
