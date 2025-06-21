using System;
using System.Collections;
using System.ComponentModel;
using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(S_PlayerMovement), typeof(Rigidbody))]
public class S_GroundMovement : MonoBehaviour, IMoveState
{
    #region Components
    private Rigidbody rb;
    private Animator anim;
    private Camera cam;

    private S_PlayerMovement playerMovement;
    private S_GrapplingMovement grapplingMovement;//see S_PlayerMovement ChangeState() note
    #endregion
    #region Movement Variables
    [Header("Ground Movement")]

    [SerializeField, Range(0, 1), Tooltip("Starting speed is a fraction of the maxgroundspeed as, 0.5 = half of maxGroundSpeed")]
    private float startingSpeedMultiplier;

    private float movingTime;

    [SerializeField, Range(0, 1), Tooltip("Percentage of movement efficiency compared to the ground movement")]
    private float airMovementMultiplier = 1f;
    private float airMultiplier = 1f;

    [SerializeField, Range(1, 20), Tooltip("Max Ground Speed Allowed ")]
    private float maxGroundSpeed;

    [SerializeField]
    private float angularSpeed;

    private Vector2 movementDirection;

    private bool jump = false;
    private bool moving = false;

    private bool doubleJumping = false;
    private bool doubleJump = false;

    private float jumpPressTime;
    #endregion
    #region Jump Variables
    [Space(2), Header("Jump Variables")]
    [SerializeField]
    private Transform groundPoint;

    [SerializeField, Range(1, 50), Tooltip("force which the player uses to jump"), Obsolete]
    private float jumpForce;

    [SerializeField]
    private float jumpSpeed;

    [SerializeField, Range(1, 1000), Tooltip("In milliseconds")]
    private float maxJumpPressTime;

    [SerializeField, Range (10, 300),Tooltip("Coyote Time in ms")]
    private float coyoteTime = 150;

    [SerializeField, Range(1, 50), Tooltip("force which the player uses to doubleJump")]
    private float doubleJumpForce;

    [SerializeField, Range(0, 90), Tooltip("Double Jump angle of movement 90 is straigth up and 0 straigth forward")]
    private float doubleJumpAngle;

    [SerializeField, Header("Gravity"), Range(0,40)]
    private float gravityForce;
    #endregion

    private Coroutine stopMovement;
    public void Attack_Perform(InputAction.CallbackContext obj)
    {
        if (grapplingMovement && grapplingMovement.enabled)
        {
            playerMovement.ChangeState(grapplingMovement);
            grapplingMovement.Attack_Perform(obj);
        }
    }

    public void Jump_Perform(InputAction.CallbackContext obj)
    {
        if (IsGrounded())
        {
            anim.SetTrigger("Jump");
            jump = true;
            jumpPressTime = 0;
        }
        else if (!doubleJumping)
            doubleJump = true;
    }

    public void Jump_Cancel(InputAction.CallbackContext obj)
    {
        jumpPressTime = 0;
        jump = false;
    }

    public void StateFixedUpdate()
    {

        ApplyGravity();

        anim.SetFloat("Speed", (rb.linearVelocity.magnitude * 100) / maxGroundSpeed);

        bool nearWall = IsNearWall();
        //Movement
        if (rb.linearVelocity.magnitude <= maxGroundSpeed && moving)
        {
            if(nearWall && !IsGrounded())
            {
                return;
            }
            if (movingTime <= 0)
                movingTime = startingSpeedMultiplier;
            else if (movingTime < 1)
                movingTime += Time.fixedDeltaTime;
            else
                movingTime = 1;

            Vector3 moveDirection = GetCameraRelativeDirection();
            Vector3 velocity = moveDirection * maxGroundSpeed * movingTime;
            velocity.y = rb.linearVelocity.y; // Preserve vertical velocity

            rb.linearVelocity = velocity;
        }
        //Jump and double jump physics
        if (jump)
        {
            jumpPressTime += Time.fixedDeltaTime * 1000;//transforming the time from seconds to millisseconds

            float jumpMultiplier = 1 - (jumpPressTime / maxJumpPressTime);
            jumpMultiplier = Mathf.Clamp(jumpMultiplier, 0.5f, 1f);

            Vector3 finalSpeed = rb.linearVelocity;
            finalSpeed.y = jumpSpeed * jumpMultiplier;

            rb.linearVelocity = finalSpeed;
            
            if(jumpPressTime >= maxJumpPressTime)
            {
                jump = false;
                jumpPressTime = 0;
            }

        }
        else if (doubleJump)
        {
            anim.SetTrigger("DoubleJump");
            float normalizedAngle = doubleJumpAngle / 90f;
            
            rb.linearVelocity = Vector3.zero;
            Vector3 doubleJumpDirection = (transform.forward * (1 - normalizedAngle)) + (transform.up * normalizedAngle);
            rb.AddForce(doubleJumpDirection.normalized * doubleJumpForce, ForceMode.Impulse);
            doubleJumping = true;
            doubleJump = false;
        }             

        if (!moving) return;

        Vector3 moveDir = GetCameraRelativeDirection();

        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, transform.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * angularSpeed);
        }
    }

    private void ApplyGravity()
    {
        if (!IsGrounded())
        {
            float accelDown = rb.linearVelocity.y - (gravityForce * Time.fixedDeltaTime);
            Vector3 gravity = rb.linearVelocity;
            gravity.y = accelDown;
            rb.linearVelocity = gravity;
            //rb.AddForce(Vector3.down * gravityForce,ForceMode.Acceleration);
        }
    }

    public void StateUpdate()
    {

    }

    public void Move_Cancel(InputAction.CallbackContext obj)
    {
        movementDirection = Vector2.zero;
        moving = false;
        movingTime = 0;
        rb.angularVelocity = Vector3.zero;
       // Debug.Log("Move_Cancel");
        if(stopMovement == null)
            stopMovement = StartCoroutine(SlowlyStopMovement());        
    }

    public void Move_Perform(InputAction.CallbackContext obj)
    {
        movementDirection = obj.ReadValue<Vector2>();
        moving = true;

        if(stopMovement != null)
        {
            StopCoroutine(stopMovement);
            stopMovement = null;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        cam = Camera.main;

        playerMovement = GetComponent<S_PlayerMovement>();
        grapplingMovement = GetComponent<S_GrapplingMovement>();
    }

    #region Private Specific Methods
    private bool IsGrounded()
    {
        bool ret = false;
        LayerMask groundMask = 1 << 6;
        Collider[] results = Physics.OverlapSphere(groundPoint.position, 0.3f, groundMask);
        if (results != null && results.Length > 0)
        {
            ret = true;
            doubleJumping = false;
            airMultiplier = 1f; //efficiency of air movement, turns 1 to not affect ground movement
        }
        else
        {
            airMultiplier = airMovementMultiplier; //on air movement
        }

        anim.SetBool("Grounded", ret);
        return ret;
    }

    private Vector3 GetCameraRelativeDirection()
    {
        if (cam == null) return Vector3.zero;

        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        // Flatten the vectors to avoid unwanted vertical movement
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();        
        return (forward * movementDirection.y + right * movementDirection.x).normalized;
    }

    private IEnumerator SlowlyStopMovement()
    {
        while (rb.linearVelocity.magnitude > 0.1f)
        {
            
            Vector3 newRate = rb.linearVelocity / 1.1f;
            newRate.y = rb.linearVelocity.y; // Preserve vertical velocity
            rb.linearVelocity = newRate;
            yield return new WaitForFixedUpdate();
        }
    }

    private bool IsNearWall()
    {
        if (movementDirection.sqrMagnitude < 0.01f)
            return false;

        Vector3 origin = transform.position + Vector3.down * 1.4f;
        Vector3 dir = GetCameraRelativeDirection();
        float distance = 1f; // Adjust as needed for your character's size

        // Only check horizontal direction
        dir.y = 0;
        dir.Normalize();

        if (dir == Vector3.zero)
            return false;
        
        return Physics.SphereCast(origin, 0.3f, dir, out _, distance, ~0, QueryTriggerInteraction.Ignore);
    }
    #endregion//Private Specific Methods

    #region Unused Methods
    public void MoveCable_Cancel(InputAction.CallbackContext obj)
    {
        //throw new System.NotImplementedException();
        return;
    }

    public void MoveCable_Perform(InputAction.CallbackContext obj)
    {
        //throw new System.NotImplementedException();
        return;
    }

    public void Attack_Cancel(InputAction.CallbackContext obj)
    {        
        return;
    }
    #endregion// Unused Methods

    public void RefreshDoubleJump()
    {
        doubleJump = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundPoint.position, 0.3f);

        Gizmos.color = Color.cyan;
        Vector3 dir = GetCameraRelativeDirection() + Vector3.down * 0.8f;
        dir.y = 0;
        dir.Normalize();
        Gizmos.DrawLine(transform.position, transform.position + dir * 1f);

        Gizmos.color = Color.blue;
        Vector3 dir2 = transform.forward * 0.8f;
        dir2.y = 0;
        dir2.Normalize();
        Gizmos.DrawLine(transform.position + Vector3.down * 1.4f, transform.position + Vector3.down * 1.4f + dir2 * 1f);
    }

#endif
    
}
