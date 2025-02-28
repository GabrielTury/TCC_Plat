using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(S_PlayerMovement), typeof(LineRenderer))]
public class S_GrapplingMovement : MonoBehaviour, IMoveState
{
    #region Components
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

    private Vector3 anchorPoint;

    private Vector3 currentHookPoint;

    [Space(2), Header("Visuals"), Range(0,5), SerializeField]
    private float lineDrawSpeed;
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
        throw new System.NotImplementedException();
    }

    public void Move_Perform(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }

    public void StateFixedUpdate()
    {
        //throw new System.NotImplementedException();
    }

    public void StateUpdate()
    {
        DrawLine();
    }

    private void StartSwing()
    {
        if(joint == null)
            joint = gameObject.AddComponent<SpringJoint>();

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxGrappleDistance, grappable))
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

    private void DrawLine()
    {
        if (!joint) return;

        currentHookPoint = Vector3.Lerp(currentHookPoint, anchorPoint, Time.deltaTime * lineDrawSpeed);

        lr.SetPosition(0, hookStart.position);
        lr.SetPosition(1, currentHookPoint);
    }

}
