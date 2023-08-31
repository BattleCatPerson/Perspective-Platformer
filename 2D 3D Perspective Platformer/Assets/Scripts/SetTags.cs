using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public class SetTags : MonoBehaviour
{
    public string tagName;
    public PhysicMaterial material;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Assign()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).tag = tagName;
            if (material != null) transform.GetChild(i).GetComponent<Collider>().material = material;
        }
    }
}
