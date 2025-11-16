using UnityEngine;
using UnityEngine.InputSystem;

public class S_ZiplineMovement : MonoBehaviour, IMoveState
{
    [Header("Detection"), Range(1, 30)]
    public float ziplineDetectionRadius = 5f;

    [Header("Visuals"), Space(2), SerializeField]
    private GameObject grapplingBelt;

    [SerializeField]
    private GameObject grapplingHand, anchorPoint, gunPoint;

    [SerializeField]
    private Vector3 bodyOffset;

    private Vector3 ogAnchorPoint;

    [Header("Movement"), Range(1, 50), SerializeField]
    private float speed = 10f;

    [SerializeField, Range(-5,5)]
    private float destinationOffset = 0f;

    private S_PlayerMovement playerMovement;

    private Vector3 destination;

    private Animator anim;

    private Rigidbody rb;

    private LineRenderer lr;

    private Vector3 movingPoint;

    private Vector2 inputStore;

    public void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;
    }

    #region Interface Methods
    public void Activation(object inputPayload = null)
    {
        anim.SetTrigger("Grapple");
        anim.SetBool("IsGrappling", true);
        float nearestDist = 0;
        Collider nearestCollider = null;
        foreach (Collider C in playerMovement.ziplinesInRange)
        {
            float dist = Vector3.Distance(transform.position, C.transform.position);

            if (nearestDist == 0 || dist < nearestDist)
            {
                nearestDist = dist;
                nearestCollider = C;
            }

        }

        S_Zipline zipline = nearestCollider.GetComponent<S_Zipline>();
        if(zipline == null)
        {
            zipline = nearestCollider.GetComponentInParent<S_Zipline>();
        }

        //GetCorrectPointFromRotation(zipline.ziplinePoints);
        destination = zipline.endPoint.position;

        movingPoint = zipline.ziplinePoints[0];
        rb.linearVelocity = Vector3.zero;

        grapplingBelt.SetActive(false);
        grapplingHand.SetActive(true);

        ogAnchorPoint = anchorPoint.transform.position;        

        lr.positionCount = 2;
    }
    public void Attack_Cancel(InputAction.CallbackContext obj)
    {
        anim.SetBool("IsGrappling", false);

        MovementInputPayload payload = new MovementInputPayload
        {
            inputDirection = inputStore
        };
        playerMovement.ChangeState(typeof(S_GroundMovement), payload);
        ExitZipline();
    }

    public void Attack_Perform(InputAction.CallbackContext obj)
    {
        
    }

    public void Jump_Cancel(InputAction.CallbackContext obj)
    {
        
    }

    public void Jump_Perform(InputAction.CallbackContext obj)
    {
        anim.SetBool("IsGrappling", false);
        MovementInputPayload payload = new MovementInputPayload
        {
            inputDirection = inputStore
        };
        playerMovement.ChangeState(typeof(S_GroundMovement), payload);
        ExitZipline();
    }

    public void MoveCable_Cancel(InputAction.CallbackContext obj)
    {
        
    }

    public void MoveCable_Perform(InputAction.CallbackContext obj)
    {
        
    }

    public void Move_Cancel(InputAction.CallbackContext obj)
    {
        inputStore = Vector2.zero;
    }

    public void Move_Perform(InputAction.CallbackContext obj)
    {
        inputStore = obj.ReadValue<Vector2>();
    }

    public void Skill_Perform(InputAction.CallbackContext obj)
    {
        anim.SetBool("IsGrappling", false);
        playerMovement.ChangeState(typeof(S_SlowedMovement));
        ExitZipline();
    }

    public void StateFixedUpdate()
    {
        transform.position = movingPoint - bodyOffset;
        anchorPoint.transform.position = movingPoint;

        Vector3 direction = destination - movingPoint;
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction, transform.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 720);
        Vector3 finalDest = destination - (Vector3.down * destinationOffset);
        movingPoint = Vector3.MoveTowards(movingPoint, finalDest, Time.fixedDeltaTime * speed);

        if(Vector3.Distance(movingPoint, finalDest) < 0.3f)
        {
            anim.SetBool("IsGrappling", false);
            MovementInputPayload payload = new MovementInputPayload
            {
                inputDirection = inputStore
            };
            playerMovement.ChangeState(typeof(S_GroundMovement), payload);
            ExitZipline();
        }

        DrawLine();
    }

    public void StateUpdate()
    {
        
    }
    #endregion //Interface Methods

    #region Auxiliary Methods

    private void DrawLine()
    {
        if (lr.positionCount < 2)
            return;

        lr.SetPosition(0, gunPoint.transform.position);
        lr.SetPosition(1, movingPoint);
    }
    private void ExitZipline()
    {
        grapplingBelt.SetActive(true);
        grapplingHand.SetActive(false);
        anchorPoint.transform.position = ogAnchorPoint;
        lr.positionCount = 0;
    }
    private void GetCorrectPointFromRotation(Vector3[] points)
    {
        float lowestAngle = 360f;        
        foreach (Vector3 point in points)
        {
            Vector3 directionToPoint = point - transform.position;
            float angle = Vector3.Angle(transform.forward, directionToPoint);
            if (lowestAngle == 360f || angle < lowestAngle) // Assuming a 90-degree field of view
            {
                lowestAngle = angle;
                destination = point;
            }
        }
    }
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovement = GetComponent<S_PlayerMovement>();
    }

}
