using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(S_PlayerMovement), typeof(LineRenderer))]
public class S_GrapplingMovement : MonoBehaviour, IMoveState
{
    
    S_PlayerMovement playerMovement;
    S_GroundMovement groundMovement;//See S_PlayerMovement ChangeState() note
    #region Components
    private Rigidbody rb;

    private SpringJoint joint;
    private LineRenderer lr;

    [Header("Components")]
    [SerializeField]
    private Transform hookStart;
    [SerializeField]
    private LayerMask grappable;
    #endregion

    #region Grappling Variables
    [Space(2), Header("Variables"), SerializeField]
    private float maxGrappleDistance;

    [SerializeField, Tooltip("The maximum distance the player can stay on the rope (this is a multiplier that takes the start position as base, so 0.8 = 80% of the distance from starting point to the attachPoint)"), Range(0, 2)]
    private float maxHangingRangeMultiplier;

    [SerializeField, Tooltip("The minimum distance the player can stay on the rope (This is a multiplier that takes the start position as base, so 0.2 = 20% of the distance from starting point to the attachPoint)"), Range(0, 1)]
    private float minHangingRangeMultiplier;

    [SerializeField]
    private float rotationSpeed;

    [InspectorLabel("Aim Assist Strength")]
    [SerializeField, Tooltip("The radius of the aim assist"), Range(0.1f, 5f)]
    private float aimRadius;

    private Vector3 anchorPoint;

    private Vector3 currentHookPoint;

    [Space(2), Header("Visuals"), Range(0,15), SerializeField]
    private float lineDrawSpeed;

    private Vector2 inputDirection;

    [InspectorLabel("Movement Swing Force")]
    [Space(2), Header("Movement"), SerializeField, Tooltip("Force which the player can mvoe while swinging")]
    private float moveForce;

    [SerializeField]
    private float maxSpeed;

    [InspectorLabel("Hook Extend Speed")]
    [SerializeField, Tooltip("Speed which the player can extend the cable")]
    private float cableSpeed;

    [InspectorLabel("Hook Retract Force")]
    [SerializeField, Tooltip("Force which the player can retract the cable")]
    private float retractForce = 5;


    [SerializeField, InspectorLabel("Innital Grappling Force"), Tooltip("Force towards the anchor point when starting the grapple")]
    private float grappleForce = 5;

    private float cableMoveDir;

    #endregion
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        lr = GetComponent<LineRenderer>(); 
        lr.positionCount = 0;
        playerMovement = GetComponent<S_PlayerMovement>();
        groundMovement = GetComponent<S_GroundMovement>();
    }
    public void Attack_Perform(InputAction.CallbackContext obj)
    {
        StartSwing();        
    }

    public void Attack_Cancel(InputAction.CallbackContext obj)
    {
        StopSwing();
        playerMovement.ChangeState(groundMovement);
    }

    public void Jump_Perform(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }

    public void Move_Cancel(InputAction.CallbackContext obj)
    {
        inputDirection = Vector3.zero;
    }

    public void Move_Perform(InputAction.CallbackContext obj)
    {
        inputDirection = obj.ReadValue<Vector2>();
    }

    public void StateFixedUpdate()
    {
        if (joint != null)
        {
            SwingMovement();
            CableMovement();
        }
    }

    public void StateUpdate()
    {
        DrawLine();

        Vector3 moveDir = GetCameraRelativeDirection();

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
#region Swing
    private void StartSwing()
    {

        Debug.Log("Before Cast");
        RaycastHit hit;        
        if (Physics.SphereCast(Camera.main.transform.position, aimRadius, Camera.main.transform.forward, out hit, maxGrappleDistance, grappable))
        {
            if (joint == null)
                joint = gameObject.AddComponent<SpringJoint>();

            anchorPoint = hit.point;
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = anchorPoint;

            float distFromPoint = Vector3.Distance(transform.position, anchorPoint);
            joint.maxDistance = distFromPoint * maxHangingRangeMultiplier;
            joint.minDistance = distFromPoint * minHangingRangeMultiplier;

            //these value might change but are better off being fixed to keep
            joint.spring = 4.5f;
            joint.damper = 7.5f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
            currentHookPoint = hookStart.position;
            Debug.Log(anchorPoint);

            rb.AddForce((anchorPoint - transform.position) * grappleForce, ForceMode.Impulse);//add movement to the player towards the grapple point
        }
        else
            StopSwing();
            //Debug.Log(hit.collider.name);

    }

    private void StopSwing()
    {
        lr.positionCount = 0;
        

        if(joint != null)
            Destroy(joint);
        else
        {
            playerMovement.ChangeState(groundMovement); //case it has no joint (case it missed)
            if (groundMovement != null)
                groundMovement.RefreshDoubleJump();
        }
    }
    #endregion//Swing
#region Swing Movement
    private void SwingMovement()
    {
        if (rb.linearVelocity.magnitude > maxSpeed) return;

        rb.AddForce(GetCameraRelativeDirection() * moveForce, ForceMode.Force);
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

        return (forward * inputDirection.y + right * inputDirection.x).normalized;
    }
    #endregion //Swing Movement
#region Cable Movement

    private void CableMovement()
    {
        if (cableMoveDir == 0) return;

        if (cableMoveDir > 0)
        {
            Vector3 dirToPoint = anchorPoint - transform.position;
            rb.AddForce(dirToPoint.normalized * retractForce, ForceMode.Acceleration);

            float distFromPoint = Vector3.Distance(transform.position, anchorPoint);

            joint.maxDistance = distFromPoint * 0.8f;
            joint.minDistance = distFromPoint * 0.25f;
        }
        else if (cableMoveDir < 0)
        {
            float distFromPoint = Vector3.Distance(transform.position, anchorPoint) + cableSpeed;

            joint.maxDistance = distFromPoint * 0.8f;
            joint.minDistance = distFromPoint * 0.25f;
        }
    }
    public void MoveCable_Perform(InputAction.CallbackContext obj)
    {
        cableMoveDir = obj.ReadValue<float>();
    }

    public void MoveCable_Cancel(InputAction.CallbackContext obj)
    {
        cableMoveDir = 0;
    }

#endregion //Cable Movement
    private void DrawLine()
    {
        if (!joint) return;

        currentHookPoint = Vector3.Lerp(currentHookPoint, anchorPoint, Time.deltaTime * lineDrawSpeed);

        lr.SetPosition(0, hookStart.position);
        lr.SetPosition(1, currentHookPoint);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Camera.main.transform.position, Camera.main.transform.forward * maxGrappleDistance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Camera.main.transform.position + (Camera.main.transform.forward * (maxGrappleDistance / 2)), aimRadius);
    }

}
