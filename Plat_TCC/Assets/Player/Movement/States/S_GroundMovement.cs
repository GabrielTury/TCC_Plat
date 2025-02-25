using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class S_GroundMovement : MonoBehaviour, IMoveState
{
    #region Components
    private Rigidbody rb;
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
    public void AttackFixedUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void Attack_Perform(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }

    public void JumpFixedUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void Jump_Perform(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }

    public void MoveFixedUpdate()
    {
        if (rb.linearVelocity.magnitude <= maxGroundSpeed && moving)
            rb.AddForce(transform.forward * accelerationForce, ForceMode.Acceleration);
    }

    public void MoveUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void Move_Cancel(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
