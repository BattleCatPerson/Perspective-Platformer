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
    public bool playerInPlatform;
    public int platformLayer;
    private int mask;
    void Start()
    {
        player = PlayerMovement.instance.transform;
        platformCollider = GetComponent<Collider>();

        mask = 1 << platformLayer;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCollider();
    }

    public void UpdateCollider()
    {
        if (playerInPlatform) return;
        bool platformEnabled = false;

        Ray ray = new Ray(player.position, Vector3.down);
        RaycastHit hit;
        bool above = Physics.Raycast(ray, out hit, Mathf.Infinity, mask) && hit.collider == raycastCollider;

        ray = new Ray(player.position, Vector3.up);
        bool below = Physics.Raycast(ray, out hit, Mathf.Infinity, mask) && hit.collider == raycastCollider;

        ray = new Ray(player.position, Vector3.right);
        bool left = Physics.Raycast(ray, out hit, Mathf.Infinity, mask) && hit.collider == raycastCollider;

        ray = new Ray(player.position, Vector3.left);
        bool right = Physics.Raycast(ray, out hit, Mathf.Infinity, mask) && hit.collider == raycastCollider;


        platformEnabled = (above && accessDirection == Direction.up) 
            || (below && accessDirection == Direction.down) 
            || (left && accessDirection == Direction.left) 
            || (right && accessDirection == Direction.right);

        platformCollider.enabled = !platformEnabled;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent == player) playerInPlatform = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent == player) playerInPlatform = false;
    }
}
