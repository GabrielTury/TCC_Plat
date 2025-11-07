using UnityEngine;
using UnityEngine.InputSystem;

public class S_ObjectMovement : MonoBehaviour, IMoveState
{
    S_PlayerMovement playerMovement;
    LineRenderer lineRenderer;
    S_GroundMovement groundMovement;

    private GameObject movable;
    private Vector2 dir;

    [SerializeField, Range(1f, 10f)]
    private float objectSpeed = 5;
    [SerializeField]
    private float objectDistance = 3;
    [SerializeField]
    private float maxObjectDistance = 6;
    [SerializeField]
    private float movableOffset = 0.5f;
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
        groundMovement.Move_Cancel(obj);
        dir = Vector2.zero;
    }

    public void Move_Perform(InputAction.CallbackContext obj)
    {
        groundMovement.Move_Perform(obj);
        dir = obj.ReadValue<Vector2>();
    }

    public void Skill_Perform(InputAction.CallbackContext obj)
    {
        playerMovement.ChangeState(typeof(S_SlowedMovement));
    }

    public void StateFixedUpdate()
    {
        groundMovement.StateFixedUpdate();

        float distanceFromObject = Vector3.Distance(movable.transform.position, transform.position);
        if (distanceFromObject < objectDistance)
            return;
        else if (distanceFromObject > maxObjectDistance)
            Attack_Cancel(new InputAction.CallbackContext());

        RaycastHit hit;
        int groundMask = 1 << 6;
        float movableYpos = movable.transform.position.y;

        if(Physics.Raycast(movable.transform.position, Vector3.down, out hit, 5f, groundMask))
        {
            movableYpos = hit.point.y + movableOffset;
        }

        Vector3 target = transform.position + ((movable.transform.position - transform.position) * 0.5f);
        Vector3 movement = Vector3.MoveTowards(movable.transform.position, target, objectSpeed/100f);
        movement.y = movableYpos;
        movable.transform.position = movement;
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
