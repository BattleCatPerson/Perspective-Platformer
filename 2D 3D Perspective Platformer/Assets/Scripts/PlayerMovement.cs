using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Dimension
{
    Two, Three
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Vector2 input;
    [SerializeField] Vector2 rawInput;
    [SerializeField] Vector2 filteredInput;

    [Header("Horizontal Movement")]
    [SerializeField] bool moving;
    [SerializeField] float speed;
    [SerializeField] float speedCap;
    [SerializeField] float sRootSpeedCap;
    [SerializeField] Transform ground;
    [SerializeField] Transform wall;
    [SerializeField] float control;
    [SerializeField] float controlThreshold;

    [Header("Jumping")]
    [SerializeField] float jumpForce;
    [SerializeField] bool grounded;

    [Header("Charged Directional Dash")]
    [SerializeField] Vector3 dashDirection;
    [SerializeField] float dashForce;
    [SerializeField] float dashCancelTime;
    [SerializeField] bool dashing;
    [SerializeField] bool canDash;
    [SerializeField] float dashCooldown;
    [SerializeField] bool dashInput;

    [Header("3D")]
    [SerializeField] Dimension dimension = Dimension.Two;
    [SerializeField] bool doubleJumped;
    [SerializeField] float doubleJumpForce;
    [SerializeField] Transform camTransform;
    [SerializeField] Transform movementDirection;

    [Header("Controller Adjustments")]
    [SerializeField, Range(0, 1)] float yThreshold;
    [SerializeField, Range(0, 1)] float xThreshold;
    void Start()
    {
        //dimension = Dimension.Two;
        grounded = true;
        canDash = true;
        control = 1;
        sRootSpeedCap = Mathf.Sqrt(Mathf.Pow(speedCap, 2) / 2);
    }

    void FixedUpdate()
    {
        if (control >= controlThreshold) rb.useGravity = true;
        if (dimension == Dimension.Two) Movement2D();
        else Movement3D();
    }

    void OnMove(InputValue value)
    {
        rawInput = value.Get<Vector2>();
        filteredInput = SortValue(rawInput);
        if (dimension == Dimension.Two) input = (new Vector2(filteredInput.x, 0)).normalized;
        else input = filteredInput;
        moving = (dimension == Dimension.Two && input.x != 0) || (dimension == Dimension.Three && (input.magnitude > 0));
    }

    void OnJump()
    {
        if (grounded)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            grounded = false;
        }
        else if (!doubleJumped && dimension == Dimension.Three)
        {
            rb.velocity = new(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * doubleJumpForce, ForceMode.Impulse);
            doubleJumped = true;
        }
    }

    void OnDash()
    {
        dashDirection = dimension == Dimension.Two ? new Vector3(filteredInput.x, filteredInput.y).normalized : new Vector3(filteredInput.x, 0, filteredInput.y);
        if (canDash && dashDirection != Vector3.zero) dashInput = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (dashing)
            {
                StopCoroutine("DashDelay");
                StopDash();
            }


            RaycastHit hit;
            if (Physics.Raycast(transform.position, -transform.up, out hit) && hit.collider.gameObject == collision.gameObject)
            {
                grounded = true;
                doubleJumped = false;
                ground = hit.collider.transform;
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
                        doubleJumped = false;
                        canDash = true;
                        return;
                    }
                }
            }

            if (Physics.Raycast(transform.position, transform.right, out hit) || Physics.Raycast(transform.position, -transform.right, out hit) && hit.collider.gameObject == collision.gameObject) wall = collision.transform;
        }
    }


    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            RaycastHit hit;

            if (dashing) return;
            if (Physics.Raycast(transform.position, -transform.up, out hit) && hit.collider.gameObject == collision.gameObject)
            {
                canDash = true;
                return;
            }
            for (int i = -1; i <= 1; i += 2)
            {
                for (int j = -1; j <= 1; j += 2)
                {
                    if (Physics.Raycast(transform.position + transform.right * transform.localScale.x / 2 * i + transform.forward * transform.localScale.z / 2 * j, -transform.up, out hit) && hit.collider.gameObject == collision.gameObject)
                    {
                        canDash = true;
                        return;
                    }
                }
            }

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform == ground)
        {
            grounded = false;
            ground = null;
        }
        if (collision.transform == wall) wall = null;
    }
    IEnumerator DashDelay()
    {
        rb.useGravity = false;
        dashing = true;
        yield return new WaitForSeconds(dashCancelTime);
        rb.useGravity = true;
        StopDash();
    }

    public void Dash()
    {
        control = 0;
        if (dimension == Dimension.Two) rb.velocity = dashDirection * dashForce;
        else rb.velocity = (movementDirection.forward * dashDirection.y + movementDirection.right * dashDirection.x) * dashForce; //fix this
        StartCoroutine("DashDelay");
        canDash = false;
        dashInput = false;
    }

    public void Movement2D()
    {
        DashControl();

        if (moving)
        {
            if (((wall != null && Mathf.Abs(transform.position.x + input.x - wall.position.x) > Mathf.Abs(transform.position.x - wall.position.x)) || wall == null) && control >= controlThreshold)
            {
                rb.AddForce(transform.right * speed * input.x * control);
                if (!dashing) rb.velocity = new(Mathf.Clamp(rb.velocity.x, -speedCap, speedCap), rb.velocity.y);
            }
        }
    }

    public void Movement3D()
    {
        DashControl();
        movementDirection.eulerAngles = new(0, camTransform.eulerAngles.y);
        if (moving)
        {
            if (((wall != null && Mathf.Abs(transform.position.x + input.x - wall.position.x) > Mathf.Abs(transform.position.x - wall.position.x)) || wall == null) && control >= controlThreshold)
            {
                rb.AddForce((movementDirection.right * input.x + movementDirection.forward * input.y).normalized * speed * control);
                if (!dashing)
                {
                    if (input.x != 0 && input.y != 0)
                    {
                        Debug.Log("1");
                        rb.velocity = new(Mathf.Clamp(rb.velocity.x, -sRootSpeedCap, sRootSpeedCap), rb.velocity.y, Mathf.Clamp(rb.velocity.z, -sRootSpeedCap, sRootSpeedCap));
                    }
                    else
                    {
                        Debug.Log("2");
                        rb.velocity = new(Mathf.Clamp(rb.velocity.x, -speedCap, speedCap), rb.velocity.y, Mathf.Clamp(rb.velocity.z, -speedCap, speedCap));
                    }
                }
            }
        }
    }

    public void DashControl()
    {
        RaycastHit hit;
        bool onWall = Physics.Raycast(transform.position, dashDirection, out hit) && (hit.collider.transform == wall || hit.collider.transform == ground);
        if (onWall)
        {
            dashInput = false;
            return;
        }

        if (dashInput && !grounded && canDash) Dash();
        else if (grounded && dashInput && canDash && dashDirection != Vector3.zero) Dash();
        if (dashing && rb.velocity.magnitude > 0.01f)
        {
            if (dimension == Dimension.Two)
            {
                if (dashDirection.y > 0)
                {
                    rb.velocity -= dashDirection * (dashForce / dashCancelTime) * Time.fixedDeltaTime;
                }
                else
                {
                    rb.velocity -= Vector3.right * dashDirection.x * (dashForce / dashCancelTime) * Time.fixedDeltaTime;
                }
            }
            else
            {
                rb.velocity -= dashDirection * (dashForce / dashCancelTime) * Time.fixedDeltaTime;
            }
            control += Time.fixedDeltaTime / dashCancelTime;
        }
        else if (dashing && rb.velocity.magnitude <= 0.1f)
        {
            StopCoroutine("DashDelay");
            StopDash();
        }
    }

    public void StopDash()
    {
        if (dimension == Dimension.Two)
        {
            if (dashDirection.y > 0)
            {
                rb.velocity = Vector3.zero;
            }
            else
            {
                rb.velocity = new(0, rb.velocity.y);
            }
        }
        else rb.velocity = Vector3.zero;
        dashing = false;
        control = 1;
    }

    public Vector2 SortValue(Vector2 v)
    {
        float multX = v.x > 0 ? 1 : -1;
        float multY = v.y > 0 ? 1 : -1;
        if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
        {
            if (v.y > 0)
            {
                if (v.y < yThreshold) return Vector2.right * multX;
                else return (new Vector2(multX, 1)).normalized;
            }
            else
            {
                if (v.y > -yThreshold) return Vector2.right * multX;
                else return (new Vector2(multX, -1)).normalized;
            }
        }
        else if (Mathf.Abs(v.y) > Mathf.Abs(v.x))
        {
            if (v.x > 0)
            {
                if (v.x < xThreshold) return Vector2.up * multY;
                else return (new Vector2(1, multY)).normalized;
            }
            else
            {
                if (v.x > -xThreshold) return Vector2.up * multY;
                else return (new Vector2(-1, multY)).normalized;
            }
        }
        return Vector2.zero;
    }
}
