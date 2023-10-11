using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Dimension
{
    Two, Three
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    [SerializeField] Rigidbody rb;
    [SerializeField] Vector2 input;
    [SerializeField] Vector2 rawInput;
    [SerializeField] Vector2 filteredInput;
    [SerializeField] Transform model;
    public Transform Model => model;
    [SerializeField] bool canMove;
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
    [SerializeField] bool jumpInput;

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
    public Dimension Dimension => dimension;
    public void ChangeDimension(Dimension d) => dimension = d;
    [SerializeField] bool doubleJumped;
    [SerializeField] float doubleJumpForce;
    [SerializeField] Transform camTransform;
    [SerializeField] Transform movementDirection;
    [SerializeField] Vector3 dashCurrentDirection;

    [Header("Controller Adjustments")]
    [SerializeField, Range(0, 1)] float yThreshold;
    [SerializeField, Range(0, 1)] float xThreshold;

    [Header("Camera")]
    [SerializeField] CameraShift cameraShift;
    public CameraShift CameraShift => cameraShift;

    [Header("Launcher")]
    [SerializeField] bool launching;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        //dimension = Dimension.Two;
        grounded = true;
        canDash = true;
        control = 1;
        canMove = true;
        sRootSpeedCap = Mathf.Sqrt(Mathf.Pow(speedCap, 2) / 2);

        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        if (!canMove || PauseMenu.paused) return;
        if (dimension == Dimension.Two) Movement2D();
        else Movement3D();
    }

    void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();
        //rawInput = value.Get<Vector2>();
        //filteredInput = SortValue(rawInput);
        //if (dimension == Dimension.Two) input = (new Vector2(filteredInput.x, 0)).normalized;
        //else input = filteredInput;
        moving = (dimension == Dimension.Two && input.x != 0) || (dimension == Dimension.Three && (input.magnitude > 0));
    }

    void OnJump()
    {
        if (dashing || !canMove || jumpForce == 0 || dashInput) return;
        jumpInput = true;
    }

    public void Jump()
    {
        if (grounded)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            grounded = false;
        }
        else if (!doubleJumped && dimension == Dimension.Three && !dashing)
        {
            rb.velocity = new(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * doubleJumpForce, ForceMode.Impulse);
            doubleJumped = true;
        }

        jumpInput = false;
    }

    void OnDash()
    {
        if (dashForce == 0 || !canMove) return;
        dashDirection = new Vector3(input.x, input.y).normalized;

        //dashDirection = new Vector3(filteredInput.x, filteredInput.y).normalized;
        if (canDash && dashDirection != Vector3.zero) dashInput = true;
    }
    void OnLaunch()
    {
        if (Launcher.currentLauncher) Launcher.currentLauncher.Launch();
    }
    private void OnCollisionEnter(Collision collision)
    {
        RaycastHit hit;
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (dashing)
            {
                if (Physics.Raycast(transform.position, dashDirection, out hit) && hit.collider == collision.collider)
                {
                    StopCoroutine("DashDelay");
                    StopDash();
                }
            }

            if (launching)
            {
                StopLaunch();
            }

            CheckGround(collision.gameObject);

            //WALLS: DEBUG FOR LAUNCHING FROM THE LAUNCHER
            if ((Physics.Raycast(transform.position, transform.right, out hit) || Physics.Raycast(transform.position, -transform.right, out hit)) && hit.collider.gameObject == collision.gameObject)
            {
                Debug.Log(transform.right);
                wall = collision.transform;
            }
        }
    }


    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            RaycastHit hit;

            if (dashing) return;
            //if (launching)
            //{
            //    StopLaunch();
            //}
            CheckGround(collision.gameObject);
            //if (Physics.Raycast(transform.position, -transform.up, out hit) && hit.collider.gameObject == collision.gameObject)
            //{
            //    canDash = true;
            //    grounded = true;
            //    return;
            //}
            //for (int i = -1; i <= 1; i += 2)
            //{
            //    for (int j = -1; j <= 1; j += 2)
            //    {
            //        if (Physics.Raycast(transform.position + transform.right * transform.localScale.x / 2 * i + transform.forward * transform.localScale.z / 2 * j, -transform.up, out hit) && hit.collider.gameObject == collision.gameObject)
            //        {
            //            canDash = true;
            //            return;
            //        }
            //    }
            //}

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
        StopDash();
    }

    public void Dash()
    {
        control = 0;
        if (dimension == Dimension.Two) rb.velocity = dashDirection * dashForce;
        else
        {
            rb.velocity = (movementDirection.forward * dashDirection.y + movementDirection.right * dashDirection.x).normalized * dashForce; //fix this
            dashCurrentDirection = (movementDirection.forward * dashDirection.y + movementDirection.right * dashDirection.x).normalized;
        }
        StartCoroutine("DashDelay");
        canDash = false;
        dashInput = false;
    }

    public void Movement2D()
    {
        JumpAndDash();
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
        JumpAndDash();
        movementDirection.eulerAngles = new(0, camTransform.eulerAngles.y);
        if (moving)
        {
            bool intoWall = false;
            if (wall)
            {
                intoWall = Vector3.Distance(transform.position, wall.position)
                > Vector3.Distance(transform.position + movementDirection.forward * input.y + movementDirection.right * input.x, wall.position);
            }

            if (((wall != null && !intoWall || wall == null) && control >= controlThreshold))
            {
                rb.AddForce((movementDirection.right * input.x + movementDirection.forward * input.y).normalized * speed * control);
                model.forward = (movementDirection.right * input.x + movementDirection.forward * input.y).normalized;
                if (!dashing)
                {
                    Vector3 vel = Vector3.ClampMagnitude(new Vector3(rb.velocity.x, 0, rb.velocity.z), sRootSpeedCap);
                    rb.velocity = new(vel.x, rb.velocity.y, vel.z);
                }
            }
        }
    }
    public void JumpAndDash()
    {
        if (((jumpInput && dashInput) || (jumpInput && !dashInput)) && ((dimension == Dimension.Two && grounded) || (dimension == Dimension.Three && !doubleJumped))) Jump();
        else DashControl();

        jumpInput = false;
        dashInput = false;
    }

    public void DashControl()
    {
        RaycastHit hit;
        bool onWall;
        if (dimension == Dimension.Two) onWall = Physics.Raycast(transform.position, dashDirection, out hit) && (hit.collider.transform == wall || (hit.collider.transform == ground));
        else onWall = Physics.Raycast(transform.position, new Vector3(dashDirection.x, 0, dashDirection.y), out hit) && (hit.collider.transform == wall || (hit.collider.transform == ground));

        if (onWall)
        {
            dashInput = false;
            return;
        }
        if ((dashInput && !grounded && canDash) || (grounded && dashInput && canDash && dashDirection != Vector3.zero)) Dash();

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
                rb.velocity -= dashCurrentDirection * (dashForce / dashCancelTime) * Time.fixedDeltaTime;
            }
            control += Time.fixedDeltaTime / dashCancelTime;
            if (control >= controlThreshold) rb.useGravity = true;
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
        if (canMove)
        {
            rb.useGravity = true;
        }
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

        return new Vector2(v.x, v.y);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + movementDirection.forward);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + movementDirection.forward * 5);
    }

    void OnQuickRotateCamera(InputValue value)
    {
        if (cameraShift) cameraShift.QuickRotate(value.Get<float>());
    }

    public void DisableMovement(bool enabled)
    {
        rb.useGravity = enabled;
        canMove = enabled;
        if (!enabled)
        {
            StopDash();
            rb.velocity = Vector3.zero;
        }
    }

    public void Launch(Vector3 direction, float velocity)
    {
        rb.velocity = direction * velocity;
        launching = true;
    }

    public void StopLaunch()
    {
        launching = false;
        rb.velocity = Vector3.zero;
        DisableMovement(true);
        model.localEulerAngles = Vector3.zero;

        //make sure wall is not set 
        //RaycastHit hit;
        //if (wall && Physics.Raycast(transform.position, transform.right, out hit) 
        //    || Physics.Raycast(transform.position, -transform.right, out hit)
        //    && hit.collider.gameObject == wall.gameObject) return;
        //wall = null;
    }

    public void CheckGround(GameObject g)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit)
                && (hit.collider.gameObject == g))
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
                if (Physics.Raycast(transform.position + transform.right * transform.localScale.x / 2 * i + transform.forward * transform.localScale.z / 2 * j, -transform.up, out hit)
                    && hit.collider.gameObject == g)
                {
                    grounded = true;
                    doubleJumped = false;
                    canDash = true;
                    ground = hit.collider.transform;
                    return;
                }
            }
        }
    }
}
