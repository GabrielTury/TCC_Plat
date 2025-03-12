using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(S_PlayerMovement), typeof(LineRenderer))]
public class S_GrapplingMovement : MonoBehaviour, IMoveState
{
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

    [InspectorLabel("Aim Assist Strength")]
    [SerializeField, Tooltip("The radius of the aim assist"), Range(0.1f, 3f)]
    private float aimRadius;

    private Vector3 anchorPoint;

    private Vector3 currentHookPoint;

    [Space(2), Header("Visuals"), Range(0,5), SerializeField]
    private float lineDrawSpeed;

    private Vector2 inputDirection;

    [InspectorLabel("Movement Swing Force")]
    [Space(2), Header("Movement"), SerializeField, Tooltip("Force which the player can mvoe while swinging")]
    private float moveForce;

    [InspectorLabel("Player Vertical Speed")]
    [SerializeField, Tooltip("Speed which the player can extend/retract the cable")]
    private float cableSpeed;

    private float cableMoveDir;

    #endregion
    private void Awake()
    {
        lr = GetComponent<LineRenderer>(); 
        lr.positionCount = 0;
    }
    public void Attack_Perform(InputAction.CallbackContext obj)
    {
        StartSwing();
    }

    public void Attack_Cancel(InputAction.CallbackContext obj)
    {
        StopSwing();
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
        
    }
#region Swing
    private void StartSwing()
    {
        if(joint == null)
            joint = gameObject.AddComponent<SpringJoint>();

        RaycastHit hit;        
        if (Physics.SphereCast(Camera.main.transform.position, aimRadius, Camera.main.transform.forward, out hit, maxGrappleDistance, grappable))
        {
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
        }

    }

    private void StopSwing()
    {
        lr.positionCount = 0;
        if(joint != null)
            Destroy(joint);
    }
    #endregion//Swing
#region Swing Movement
    private void SwingMovement()
    {
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
            rb.AddForce(dirToPoint.normalized * moveForce, ForceMode.Force);

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
