using UnityEngine;
using UnityEngine.InputSystem;

public class S_PlayerMovement : MonoBehaviour
{
    //[SerializeField]
    //private GameObject blobShadow;

    private InputSystem_Actions inputs;

    private IMoveState[] moveStates;
    private IMoveState activeState;

    private bool canGrapple;
    private S_GrapplingMovement grapplingMovement;

    public Collider[] grapplingCollidersInRange { get; private set; }

    private void Awake()
    {
        inputs = new InputSystem_Actions();
        moveStates = GetComponents<IMoveState>();
        activeState = moveStates[0];

        foreach (IMoveState moveState in moveStates)
        {
            if (moveState is S_GrapplingMovement)
            {
                canGrapple = true;
                grapplingMovement = (S_GrapplingMovement)moveState;
                Debug.Log("Has Grapple ability");
            }
        }
    }
    private void OnEnable()
    {
        inputs.Enable();
        inputs.Player.Move.performed += Move_performed;
        inputs.Player.Move.canceled += Move_canceled;
        inputs.Player.Jump.performed += Jump_performed;
        inputs.Player.Jump.canceled += Jump_canceled;
        inputs.Player.Attack.performed += Attack_performed;
        inputs.Player.Attack.canceled += Attack_canceled;
        inputs.Player.MoveCable.performed += MoveCable_performed;
        inputs.Player.MoveCable.canceled += MoveCable_canceled;
    }

    private void Jump_canceled(InputAction.CallbackContext obj)
    {
        activeState.Jump_Cancel(obj);
    }

    private void MoveCable_canceled(InputAction.CallbackContext obj)
    {
        activeState.MoveCable_Cancel(obj);
    }

    private void MoveCable_performed(InputAction.CallbackContext obj)
    {
        activeState.MoveCable_Perform(obj);
    }

    private void Attack_canceled(InputAction.CallbackContext obj)
    {
        activeState.Attack_Cancel(obj);
    }

    private void Attack_performed(InputAction.CallbackContext obj)
    {
        activeState.Attack_Perform(obj);
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        activeState.Jump_Perform(obj);
    }

    private void OnDisable()
    {
        inputs.Disable();
        inputs.Player.Move.performed -= Move_performed;
        inputs.Player.Move.canceled -= Move_canceled;
        inputs.Player.Jump.performed -= Jump_performed;
        inputs.Player.Jump.canceled -= Jump_canceled;
        inputs.Player.Attack.performed -= Attack_performed;
        inputs.Player.Attack.canceled -= Attack_canceled;
        inputs.Player.MoveCable.performed -= MoveCable_performed;
        inputs.Player.MoveCable.canceled -= MoveCable_canceled;
    }

    private void Move_canceled(InputAction.CallbackContext obj)
    {
        activeState.Move_Cancel(obj);
    }

    private void Move_performed(InputAction.CallbackContext obj)
    {

        activeState.Move_Perform(obj);
    }

    void Start()
    {
        
    }

    private void FixedUpdate()
    {      
        activeState.StateFixedUpdate();
        GrapplingRange();
        BlobShadow();
    }
    // Update is called once per frame
    void Update()
    {
        activeState.StateUpdate();
    }

    private void BlobShadow()
    {
        /*RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100f, LayerMask.GetMask("Ground"), QueryTriggerInteraction.Ignore))
        {
            Vector3 shadowPosition = hit.point;
            shadowPosition.y += 0.01f;
            blobShadow.transform.position = shadowPosition;
        }
        else
            Debug.Log("No hit on shadow");*/
    }

    private void GrapplingRange()
    {
        if (canGrapple)
        {
            Collider[] objects = Physics.OverlapSphere(transform.position, grapplingMovement.grappleDetectionRange, 1 << 7);
            if (objects != null)
            {
                grapplingCollidersInRange = objects;
                foreach (Collider collider in objects)
                {
                    collider.SendMessage("InRange", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }

    public void ChangeState(IMoveState state)//change this to work with a enum
    {
        foreach (IMoveState m in moveStates)
        {
            if (m == state)
            {
                Debug.Log("Changed State to: "+ state.ToString());
                activeState = state;
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 100f);
    }
}
