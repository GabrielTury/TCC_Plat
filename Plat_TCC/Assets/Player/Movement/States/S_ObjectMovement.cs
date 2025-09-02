using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

public class S_ObjectMovement : MonoBehaviour, IMoveState
{
    S_PlayerMovement playerMovement;
    LineRenderer lineRenderer;
    S_GroundMovement groundMovement;

    private GameObject movable;
    private Vector2 dir;

    [SerializeField]
    private float objectSpeed = 5;
    #region Interface Methods
    public void Activation()
    {
        float nearestDist = 0;
        Collider nearestCollider = null;
        foreach (Collider col in playerMovement.movablesInRange)
        {
            float dist = Vector3.Distance(transform.position, col.transform.position);

            if (nearestDist == 0 || dist < nearestDist)
            {
                nearestDist = dist;
                nearestCollider = col;
            }
        }
        movable = nearestCollider.gameObject;

        lineRenderer.positionCount = 2;
    }

    public void Attack_Cancel(InputAction.CallbackContext obj)
    {
        playerMovement.ChangeState(typeof(S_GroundMovement));
        lineRenderer.positionCount = 0;
    }

    public void Attack_Perform(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }

    public void Jump_Cancel(InputAction.CallbackContext obj)
    {
        
    }

    public void Jump_Perform(InputAction.CallbackContext obj)
    {
        
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
        groundMovement.MoveCable_Cancel(obj);
        dir = Vector2.zero;
    }

    public void Move_Perform(InputAction.CallbackContext obj)
    {
        groundMovement.Move_Perform(obj);
        dir = obj.ReadValue<Vector2>();
    }

    public void StateFixedUpdate()
    {
        groundMovement.StateFixedUpdate();
        Vector2 movement = dir * objectSpeed * Time.fixedDeltaTime;
        movable.transform.position += new Vector3(movement.x, 0, movement.y);

    }

    public void StateUpdate()
    {
        Vector3 pos = movable.transform.position;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, pos);
    }
    #endregion // Interface Methods

    private void Awake()
    {
        playerMovement = GetComponent<S_PlayerMovement>();
        groundMovement = GetComponent<S_GroundMovement>();
        lineRenderer = GetComponent<LineRenderer>();
    }
}
