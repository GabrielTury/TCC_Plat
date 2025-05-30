using System;
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
    private Animator anim;
    private Camera cam;

    private SpringJoint joint;
    private LineRenderer lr;

    [Header("Components")]
    [SerializeField]
    private Transform hookStart;
    [SerializeField]
    private LayerMask grappable;
    #endregion

    #region Grappling Variables

    [Header("Temporary"), Obsolete]
    public bool enableObsoleteGrappling;

    [SerializeField, Tooltip("The maximum distance the player can stay on the rope (this is a multiplier that takes the start position as base, so 0.8 = 80% of the distance from starting point to the attachPoint)"), Range(0, 2)]
    private float maxHangingRangeMultiplier;

    [SerializeField, Tooltip("The minimum distance the player can stay on the rope (This is a multiplier that takes the start position as base, so 0.2 = 20% of the distance from starting point to the attachPoint)"), Range(0, 1)]
    private float minHangingRangeMultiplier;

    [SerializeField]
    private float rotationSpeed;

    [InspectorLabel("Max Angle from Player")]
    [SerializeField, Tooltip("The radius of the aim assist"), Range(1, 90f)]
    private float maxAngle;

    [InspectorLabel("Grapple Detection Range")]
    [Tooltip("The distance at which grapple objects will be detected")]
    public float grappleDetectionRange;

    private Vector3 anchorPoint;

    private Vector3 currentHookPoint;

    [Space(2), Header("Visuals"), Range(0,15), SerializeField]
    private float lineDrawSpeed;

    private Vector2 inputDirection;

    private Vector2 lastinputDir;

    [InspectorLabel("Movement Swing Force")]
    [Space(2), Header("Grapple Movement"), SerializeField, Tooltip("Force which the player can mvoe while swinging")]
    private float moveForce;

    [SerializeField]
    private float grappleForceCooldown;

    private float grappleTimer = 0;

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

    private Collider closestCollider;

    #endregion
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        lr = GetComponent<LineRenderer>(); 
        cam = Camera.main;

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

    public void Jump_Cancel(InputAction.CallbackContext obj)
    {
        throw new NotImplementedException();
    }

    public void Move_Cancel(InputAction.CallbackContext obj)
    {
        inputDirection = Vector3.zero;
    }

    public void Move_Perform(InputAction.CallbackContext obj)
    {
        inputDirection = obj.ReadValue<Vector2>();

        if(lastinputDir == Vector2.zero)
            lastinputDir = inputDirection * -1;

        /*float dotProd = Vector3.Dot(inputDirection, lastinputDir);

        if ( dotProd < 0.5 && grappleTimer >= grappleForceCooldown)
        {
            rb.AddForce(GetCameraRelativeDirection() * moveForce, ForceMode.Impulse);
            grappleTimer = 0;
            lastinputDir = inputDirection;
        }*/

    }

    public void StateFixedUpdate()
    {
        if (joint != null)
        {
            SwingMovement();
            CableMovement();

            //Count Timer
            if(grappleTimer < grappleForceCooldown)
                grappleTimer += Time.fixedDeltaTime;

            anchorPoint = closestCollider.transform.position;

            joint.connectedAnchor = anchorPoint;


            Vector3 moveDir = GetCameraRelativeDirection();

            if (moveDir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }

            rb.AddForce(Vector3.down * 10, ForceMode.Acceleration);
        }
    }

    public void StateUpdate()
    {
        DrawLine();

    }
    #region Swing
    private void StartSwing()
    {
        if (playerMovement.grapplingCollidersInRange.Length != 0)
        {
            closestCollider = null;
            float closestDistance = float.MaxValue;
            Vector3 playerPosition = transform.position;
            Vector3 playerForward = transform.forward;

            foreach (var collider in playerMovement.grapplingCollidersInRange)
            {
                Vector3 directionToCollider = (collider.transform.position - playerPosition).normalized;
                float angle = Vector3.Angle(playerForward, directionToCollider);

                if (angle <= maxAngle)
                {
                    float distance = Vector3.Distance(playerPosition, collider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestCollider = collider;
                    }
                }
                else
                {
                    Debug.Log("Angle too high");
                }
            }

            if (closestCollider != null)
            {
                anim.SetTrigger("Grapple");
                anim.SetBool("IsGrappling", true);

                if (joint == null)
                        joint = gameObject.AddComponent<SpringJoint>();

                    anchorPoint = closestCollider.transform.position;
                    joint.autoConfigureConnectedAnchor = false;
                    joint.connectedAnchor = anchorPoint;

                    float distFromPoint = Vector3.Distance(transform.position, anchorPoint);
                    joint.maxDistance = distFromPoint * maxHangingRangeMultiplier;
                    joint.minDistance = distFromPoint * minHangingRangeMultiplier;

                    joint.spring = 9.5f;
                    joint.damper = 7.5f;
                    joint.massScale = 4.5f;

                    lr.positionCount = 2;
                    currentHookPoint = hookStart.position;

                    rb.AddForce((anchorPoint - transform.position) * grappleForce, ForceMode.Impulse);                    
            }
            else
            {
                StopSwing();
            }
        }
        else
        {
            StopSwing();
        }
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

        anim.SetBool("IsGrappling", false);
    }
    private void SwingMovement()
    {
        if (inputDirection != Vector2.zero)
        {
            float dotProd = Vector3.Dot(inputDirection, lastinputDir);

            if(dotProd < 0.5)
                grappleTimer = 0;

            if (grappleTimer <= grappleForceCooldown)
            {
                rb.AddForce(GetCameraRelativeDirection() * moveForce, ForceMode.Force);
                /*grappleTimer = 0;*/
                lastinputDir = inputDirection;
            }
        }

        /*if (enableObsoleteGrappling)
        {
            if (rb.linearVelocity.magnitude > maxSpeed) return;

            rb.AddForce(GetCameraRelativeDirection() * moveForce, ForceMode.Force);
        }*/
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
        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(cam.transform.position, cam.transform.forward * maxGrappleDistance);
        //Gizmos.color = Color.cyan;
        //Gizmos.DrawWireSphere(cam.transform.position + (cam.transform.forward * (maxGrappleDistance / 2)), aimRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, grappleDetectionRange);
    }

}
