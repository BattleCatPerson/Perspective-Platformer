using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    left, right, up, down
}

public class OneWayPlatform : MonoBehaviour
{
    public Direction accessDirection;
    public Transform player;
    public Collider platformCollider;
    public Collider raycastCollider;
    void Start()
    {
        player = PlayerMovement.instance.transform;
        platformCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCollider();
    }

    public void UpdateCollider()
    {
        bool platformEnabled = false;

        Ray ray = new Ray(player.position, Vector3.down);
        RaycastHit hit;
        bool above = Physics.Raycast(ray, out hit) && hit.collider == raycastCollider;

        ray = new Ray(player.position, Vector3.up);
        bool below = Physics.Raycast(ray, out hit) && hit.collider == raycastCollider;

        ray = new Ray(player.position, Vector3.right);
        bool left = Physics.Raycast(ray, out hit) && hit.collider == raycastCollider;

        ray = new Ray(player.position, Vector3.left);
        bool right = Physics.Raycast(ray, out hit) && hit.collider == raycastCollider;

        Debug.Log($"above {above} below {below} left {left} right {right}");
    }
}
