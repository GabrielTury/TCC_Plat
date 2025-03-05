using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(S_PlayerMovement), typeof(Rigidbody))]
public class S_GroundMovement : MonoBehaviour, IMoveState
{
    #region Components
    private Rigidbody rb;

    private S_PlayerMovement playerMovement;
    private S_GrapplingMovement grapplingMovement;
    #endregion
    #region Movement Variables
    [Header("Ground Movement")]
    [SerializeField, Range(1, 50), Tooltip("Rate at which the player will accelerate when moving")]
    private float accelerationForce;

    [SerializeField, Range(1, 20), Tooltip("Max Ground Speed Allowed ")]
    private float maxGroundSpeed;

    [SerializeField]
    private float angularSpeed;

    private Vector2 movementDirection;

    private bool jump = false;
    private bool moving = false;

    private bool doubleJumping = false;
    private bool doubleJump = false;
    #endregion
    #region Jump Variables
    [Space(2), Header("Jump Variables")]
    [SerializeField]
    private Transform groundPoint;

    [SerializeField, Range(1, 50), Tooltip("force which the player uses to jump")]
    private float jumpForce;

    [SerializeField, Range(1, 50), Tooltip("force which the player uses to doubleJump")]
    private float doubleJumpForce;
    #endregion
    public void Attack_Perform(InputAction.CallbackContext obj)
    {
        if (grapplingMovement)
            playerMovement.ChangeState(grapplingMovement);
    }

    public void Jump_Perform(InputAction.CallbackContext obj)
    {
        if (IsGrounded())
            jump = true;
        else if (!doubleJumping)
            doubleJump = true;
    }

    public void StateFixedUpdate()
    {
        if (rb.linearVelocity.magnitude <= maxGroundSpeed && moving)
        {
            Vector3 moveDirection = GetCameraRelativeDirection();
            rb.AddForce(moveDirection * accelerationForce, ForceMode.Acceleration);
        }

        if (jump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jump = false;
        }
        else if (doubleJump)
        {
            rb.linearVelocity = Vector3.zero;
            Vector3 doubleJumpDirection = (transform.forward * 1f) + (transform.up * 0.5f);
            rb.AddForce(doubleJumpDirection.normalized * doubleJumpForce, ForceMode.Impulse);
            doubleJumping = true;
            doubleJump = false;
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
    }

    public void Move_Perform(InputAction.CallbackContext obj)
    {
        movementDirection = obj.ReadValue<Vector2>();
        moving = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
        }

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
    #endregion//Private Specific Methods
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundPoint.position, 0.5f);
    }

    public void Attack_Cancel(InputAction.CallbackContext obj)
    {
        Debug.Log("(S_GroundMovement::Attack_Cancel) This does nothing and should not be called");
        return;
    }
#endif
    //@todo colocar gravidade extra, alterar o controle do personagem quando no ar, tempo de parada após perder input
}
