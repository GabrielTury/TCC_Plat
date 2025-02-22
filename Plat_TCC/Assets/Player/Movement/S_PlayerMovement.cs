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

    private InputSystem_Actions inputs;

    private Rigidbody rb;

    private Vector2 movementDirection;

    private bool jumping = false;
    private bool jump = false;

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
        if (!jumping)
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
        Debug.Log("Canceled");
        movementDirection = Vector2.zero;
    }

    private void Move_performed(InputAction.CallbackContext obj)
    {
        Debug.Log(obj.ReadValue<Vector2>());
        movementDirection = obj.ReadValue<Vector2>();
    }

    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        rb.AddForce(new Vector3(movementDirection.x, 0, movementDirection.y) * accelerationForce, ForceMode.Acceleration);

        if (jump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumping = true;
            jump = false;
        }
        else if (doubleJump)
        {
            rb.AddForce(new Vector3(0, 0.5f, 0.5f) * doubleJumpForce, ForceMode.Impulse);
            doubleJumping = true;
            doubleJump = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
