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
    [SerializeField] Vector3 mousePosition;
    [SerializeField] float dashForce;
    [SerializeField] float currentChargeTime;
    [SerializeField] float chargeTime;
    [SerializeField] bool charging;
    [SerializeField] float dashCancelTime;
    [SerializeField] bool dashing;
    private bool moving;
    void Start()
    {
        grounded = true;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            mousePosition = hit.point;
        }

        if (charging)
        {
            currentChargeTime += Time.deltaTime;
            currentChargeTime = Mathf.Clamp(currentChargeTime, 0, chargeTime);
            rb.velocity = Vector3.zero;
        }
        else
        {
            if (currentChargeTime > 0)
            {
                Debug.Log("Dash");
                rb.AddForce((mousePosition - transform.position).normalized * dashForce * (currentChargeTime / chargeTime), ForceMode.Impulse);
                currentChargeTime = 0;
                StartCoroutine(DashDelay());
            }

            if (dashing) return;

            if (moving)
            {
                //rb.velocity = speed * input.x * transform.right + transform.up * rb.velocity.y;
                rb.AddForce(transform.right * speed * input.x);
                if (!rotated) rb.velocity = new(Mathf.Clamp(rb.velocity.x, -speedCap, speedCap), rb.velocity.y);
                else rb.velocity = new(rb.velocity.x, rb.velocity.y, Mathf.Clamp(rb.velocity.z, -speedCap, speedCap));

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

    void OnDash(InputValue value)
    {
        charging = value.Get<float>() == 1;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (Physics.Raycast(transform.position, -transform.up))
            {
                grounded = true;
                return;
            }
            for (int i = -1; i <= 1; i += 2)
            {
                for (int j = -1; j <= 1; j += 2)
                {
                    if (Physics.Raycast(transform.position + transform.right * transform.localScale.x / 2 * i + transform.forward * transform.localScale.z / 2 * j, -transform.up))
                    {
                        grounded = true;
                        return;
                    }
                }
            }
        }
    }

    IEnumerator DashDelay()
    {
        dashing = true;
        yield return new WaitForSeconds(dashCancelTime);
        dashing = false;
    }
}
