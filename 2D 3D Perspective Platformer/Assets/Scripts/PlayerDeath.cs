using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] Transform respawnPoint;
    [SerializeField] Rigidbody rb;
    [SerializeField] float respawnDelay;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        transform.position = respawnPoint.position;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Hazard"))
        {
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn()
    {
        EnableDisableComponents(false);
        yield return new WaitForSeconds(respawnDelay);
        Restart();
    }

    public void EnableDisableComponents(bool b)
    {
        GetComponentInChildren<Renderer>().enabled = b;
        rb.isKinematic = !b;
    }

    public void Restart()
    {
        transform.position = respawnPoint.position;
        rb.velocity = Vector3.zero;
        EnableDisableComponents(true);
    }
}
