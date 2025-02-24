using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.InputSystem;

public class S_PlayerMovement : MonoBehaviour
{
    [SerializeField, Range(1,50), Tooltip("Rate at which the player will accelerate when moving")]
    private float accelerationForce;
    [SerializeField, Range(1, 50), Tooltip("force which the player uses to jump")]
    private float jumpForce;
    [SerializeField, Range(1, 50), Tooltip("force which the player uses to doubleJump")]
    private float doubleJumpForce;

    [SerializeField, Range(1, 20), Tooltip("Max Ground Speed Allowed ")]
    private float maxGroundSpeed;

    [SerializeField]
    private float angularSpeed;

    [SerializeField]
    private Transform groundPoint;

    private InputSystem_Actions inputs;

    private Rigidbody rb;

    private Vector2 movementDirection;

    private bool jumping = false;
    private bool jump = false;
    private bool moving = false;

    private bool doubleJumping = false;
    private bool doubleJump = false;    

    private void Awake()
    {
        inputs = new InputSystem_Actions();
        rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        inputs.Enable();
        inputs.Player.Move.performed += Move_performed;
        inputs.Player.Move.canceled += Move_canceled;
        inputs.Player.Jump.performed += Jump_performed;
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        if (IsGrounded())
            jump = true;
        else if (!doubleJumping)
            doubleJump = true;
    }

    private void OnDisable()
    {
        inputs.Disable();
        inputs.Player.Move.performed -= Move_performed;
        inputs.Player.Move.canceled -= Move_canceled;
    }

    private void Move_canceled(InputAction.CallbackContext obj)
    {
        //Debug.Log("Canceled");
        movementDirection = Vector2.zero;
        moving = false;
    }

    private void Move_performed(InputAction.CallbackContext obj)
    {
        //Debug.Log(obj.ReadValue<Vector2>());
        movementDirection = obj.ReadValue<Vector2>();
        moving = true;
    }

    void Start()
    {
        
    }

    private void FixedUpdate()
    {        
        if(rb.linearVelocity.magnitude <= maxGroundSpeed && moving)
            rb.AddForce(transform.forward * accelerationForce, ForceMode.Acceleration);

        

        if (jump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumping = true;
            jump = false;
        }
        else if (doubleJump)
        {
            Vector3 doubleJumpDirection = (transform.forward * 1f) + (transform.up * 0.5f);
            rb.AddForce(doubleJumpDirection.normalized * doubleJumpForce, ForceMode.Impulse);
            doubleJumping = true;
            doubleJump = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!moving) return;

        Vector3 moveDir = new Vector3(movementDirection.x, 0 , movementDirection.y);
        Quaternion targetRotation = Quaternion.LookRotation(moveDir);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * angularSpeed);
    }

    bool IsGrounded()
    {
        bool ret = false;
        LayerMask groundMask = 1<<6;
        Collider[] results = Physics.OverlapSphere(groundPoint.position, 0.5f, groundMask);
        if (results != null && results.Length > 0)
        {
            ret = true;
            jumping = false;
            doubleJumping = false;
        }

        return ret;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundPoint.position, 0.5f);
    }
}
