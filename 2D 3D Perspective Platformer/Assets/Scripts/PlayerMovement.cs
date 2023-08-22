using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Vector2 input;

    [Header("Horizontal Movement")]
    [SerializeField] float speed;

    [Header("Jumping")]
    [SerializeField] float jumpForce;
    [SerializeField] bool grounded;

    [Header("Camera Rotation")]
    [SerializeField] bool rotated;
    [SerializeField] float rotationTime;
    [SerializeField] float currentRotationTime;
    [SerializeField] bool currentlyRotating;
    [SerializeField] Quaternion currentRotation;

    private bool moving;
    void Start()
    {
        grounded = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            rb.velocity = speed * input.x * transform.right + transform.up * rb.velocity.y;
        }
        else
        {
            rb.velocity = new(0, rb.velocity.y);
        }

        if (currentlyRotating)
        {
            if (!rotated) currentRotation = Quaternion.Euler(0, Mathf.Lerp(0, -90, currentRotationTime / rotationTime), 0);
            else currentRotation = Quaternion.Euler(0, Mathf.Lerp(-90, 0, currentRotationTime / rotationTime), 0);
            currentRotationTime += Time.deltaTime;

            if (currentRotationTime >= rotationTime)
            {
                currentRotationTime = 0;
                rotated = !rotated;
                currentlyRotating = false;
            }

            transform.rotation = currentRotation;
        }
    }

    void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();
        moving = input.x != 0;
    }

    void OnJump()
    {
        if (grounded)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            grounded = false;
        }
    }

    void OnRotateCamera()
    {
        if (!currentlyRotating) currentlyRotating = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && Physics.Raycast(transform.position, -transform.up)) grounded = true;
    }
}
