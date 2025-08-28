using UnityEngine;
using UnityEngine.InputSystem;

public class S_ZiplineMovement : MonoBehaviour, IMoveState
{
    [Header("Detection"), Range(1, 30)]
    public float ziplineDetectionRadius = 5f;

    private S_PlayerMovement playerMovement;

    private S_Zipline zipline;

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

        zipline = nearestCollider.GetComponent<S_Zipline>();

        Debug.Log("Activated Zipline Code");
    }
    public void Attack_Cancel(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }

    public void Attack_Perform(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }

    public void Jump_Cancel(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }

    public void Jump_Perform(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }

    public void MoveCable_Cancel(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }

    public void MoveCable_Perform(InputAction.CallbackContext obj)
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
        //zipline.
    }

    public void StateUpdate()
    {
        throw new System.NotImplementedException();
    }
    #endregion //Interface Methods

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovement = GetComponent<S_PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
