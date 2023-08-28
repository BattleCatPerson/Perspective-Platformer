using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Vector2 input;

    [Header("Horizontal Movement")]
    [SerializeField] float speed;
    [SerializeField] float speedCap;
    [SerializeField] Transform wall;
    [SerializeField] float control;

    [Header("Jumping")]
    [SerializeField] float jumpForce;
    [SerializeField] bool grounded;

    [Header("Camera Rotation")]
    [SerializeField] bool rotated;
    [SerializeField] float rotationTime;
    [SerializeField] float currentRotationTime;
    [SerializeField] bool currentlyRotating;
    [SerializeField] Quaternion currentRotation;

    [Header("Charged Directional Dash")]
    [SerializeField] Vector3 dashDirection;
    [SerializeField] float dashForce;
    [SerializeField] float dashCancelTime;
    [SerializeField] bool dashing;
    [SerializeField] bool canDash;
    [SerializeField] float dashCooldown;
    private bool moving;
    void Start()
    {
        grounded = true;
        canDash = true;
        control = 1;
    }

    // Update is called once per frame
    void Update()
    {

        if (dashing && rb.velocity.magnitude > 0.1f)
        {
            rb.velocity -= dashDirection * (dashForce / dashCancelTime) * Time.deltaTime;
            control += Time.deltaTime / dashCancelTime;
        }

        if (moving)
        {
            //rb.velocity = speed * input.x * transform.right + transform.up * rb.velocity.y;
            if (wall != null && Mathf.Abs(transform.position.x + input.x - wall.position.x) > Mathf.Abs(transform.position.x - wall.position.x) || wall == null)
            {
                rb.AddForce(transform.right * speed * input.x * control);
                if (!rotated && !dashing) rb.velocity = new(Mathf.Clamp(rb.velocity.x, -speedCap, speedCap), rb.velocity.y);
                //else rb.velocity = new(rb.velocity.x, rb.velocity.y, Mathf.Clamp(rb.velocity.z, -speedCap, speedCap));
            }
        }
        //else
        //{
        //    rb.velocity = new(0, rb.velocity.y);
        //}


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

    void OnDash()
    {
        if (!canDash) return;
        dashDirection = new Vector3(input.x, input.y).normalized;
        if (dashDirection == Vector3.zero) return;
        control = 0;
        rb.velocity = dashDirection * dashForce;
        StartCoroutine("DashDelay");
        canDash = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (dashing)
            {
                StopCoroutine("DashDelay");
                control = 1;
                dashing = false;
            }
            
            
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -transform.up, out hit) && hit.collider.gameObject == collision.gameObject)
            {
                grounded = true;
                canDash = true;
                return;
            }
            for (int i = -1; i <= 1; i += 2)
            {
                for (int j = -1; j <= 1; j += 2)
                {
                    if (Physics.Raycast(transform.position + transform.right * transform.localScale.x / 2 * i + transform.forward * transform.localScale.z / 2 * j, -transform.up, out hit) && hit.collider.gameObject == collision.gameObject)
                    {
                        grounded = true;
                        canDash = true;
                        return;
                    }
                }
            }

            if (Physics.Raycast(transform.position, transform.right, out hit) || Physics.Raycast(transform.position, -transform.right, out hit) && hit.collider.gameObject == collision.gameObject) wall = collision.transform;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = false;
            wall = null;
            rb.useGravity = true;
        }
    }
    IEnumerator DashDelay()
    {
        rb.useGravity = false;
        dashing = true;
        yield return new WaitForSeconds(dashCancelTime);
        rb.useGravity = true;
        dashing = false;
    }
}
