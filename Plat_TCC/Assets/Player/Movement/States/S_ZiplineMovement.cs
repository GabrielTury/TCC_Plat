using UnityEngine;
using UnityEngine.InputSystem;

public class S_ZiplineMovement : MonoBehaviour, IMoveState
{
    [Header("Detection"), Range(1, 30)]
    public float ziplineDetectionRadius = 5f;

    [Header("Movement"), Range(1, 50), SerializeField]
    private float speed = 10f;

    [SerializeField, Range(-5,5)]
    private float destinationOffset = 0f;

    private S_PlayerMovement playerMovement;

    private Vector3 destination;

    private Animator anim;

    private Rigidbody rb;

    public void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    #region Interface Methods
    public void Activation()
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

        transform.position = zipline.ziplinePoints[0];
        rb.linearVelocity = Vector3.zero;

        //Debug.Log("Activated Zipline Code");
    }
    public void Attack_Cancel(InputAction.CallbackContext obj)
    {
        anim.SetBool("IsGrappling", false);
        playerMovement.ChangeState(typeof(S_GroundMovement));
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
        playerMovement.ChangeState(typeof(S_GroundMovement));
    }

    public void MoveCable_Cancel(InputAction.CallbackContext obj)
    {
        
    }

    public void MoveCable_Perform(InputAction.CallbackContext obj)
    {
        
    }

    public void Move_Cancel(InputAction.CallbackContext obj)
    {
        
    }

    public void Move_Perform(InputAction.CallbackContext obj)
    {
       
    }

    public void Skill_Perform(InputAction.CallbackContext obj)
    {
        anim.SetBool("IsGrappling", false);
        playerMovement.ChangeState(typeof(S_SlowedMovement));
    }

    public void StateFixedUpdate()
    {
        Vector3 direction = destination - transform.position;
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction, transform.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 720);
        Vector3 finalDest = destination - (Vector3.down * destinationOffset);
        transform.position = Vector3.MoveTowards(transform.position, finalDest, Time.fixedDeltaTime * speed);

        if(Vector3.Distance(transform.position, finalDest) < 0.3f)
        {
            anim.SetBool("IsGrappling", false);
            playerMovement.ChangeState(typeof(S_GroundMovement));
        }
    }

    public void StateUpdate()
    {
        
    }
    #endregion //Interface Methods

    #region Auxiliary Methods
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
