using UnityEngine;
using UnityEngine.InputSystem;

public class S_ZiplineMovement : MonoBehaviour, IMoveState
{
    [Header("Detection"), Range(1, 30)]
    public float ziplineDetectionRadius = 5f;

    [Header("Movement"), Range(1, 50), SerializeField]
    private float speed = 10f;

    private S_PlayerMovement playerMovement;

    private Vector3 destination;

    #region Interface Methods
    public void Activation()
    {
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

        GetCorrectPointFromRotation(zipline.ziplinePoints);

        

        Debug.Log("Activated Zipline Code");
    }
    public void Attack_Cancel(InputAction.CallbackContext obj)
    {
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

    public void StateFixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, Time.fixedDeltaTime * speed);
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
