using System;
using System.Collections;
using System.ComponentModel;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(S_PlayerMovement), typeof(Rigidbody))]
public class S_GroundMovement : MonoBehaviour, IMoveState
{
    #region Components
    private Rigidbody rb;
    private Animator anim;

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
        if (grapplingMovement)
        {
            playerMovement.ChangeState(grapplingMovement);
            grapplingMovement.Attack_Perform(obj);
        }
    }

    public void Jump_Perform(InputAction.CallbackContext obj)
    {
        if (IsGrounded())
        {
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
        anim.SetFloat("Speed", (rb.linearVelocity.magnitude * 100) / maxGroundSpeed);
        //Ground Movement
        if (rb.linearVelocity.magnitude <= maxGroundSpeed && moving)
        {
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
            anim.SetTrigger("Jump"); //Change place
            
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
        
        if(!IsGrounded())
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
        if (!moving) return;

        Vector3 moveDir = GetCameraRelativeDirection();

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * angularSpeed);
        }
    }

    public void Move_Cancel(InputAction.CallbackContext obj)
    {
        movementDirection = Vector2.zero;
        moving = false;
        movingTime = 0;
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

        playerMovement = GetComponent<S_PlayerMovement>();
        grapplingMovement = GetComponent<S_GrapplingMovement>();
    }

    #region Private Specific Methods
    private bool IsGrounded()
    {
        bool ret = false;
        LayerMask groundMask = 1 << 6;
        Collider[] results = Physics.OverlapSphere(groundPoint.position, 0.5f, groundMask);
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
        if (Camera.main == null) return Vector3.zero;

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

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
            rb.linearVelocity = rb.linearVelocity / 1.1f;
            yield return new WaitForFixedUpdate();
        }
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
        Gizmos.DrawWireSphere(groundPoint.position, 0.5f);
    }

#endif
    //@todo colocar gravidade extra, alterar o controle do personagem quando no ar, tempo de parada após perder input
}
