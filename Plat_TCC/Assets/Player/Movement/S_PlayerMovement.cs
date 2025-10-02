using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class S_PlayerMovement : MonoBehaviour
{
    //[SerializeField]
    //private GameObject blobShadow;
    public bool bSlowedTime;

    //private Animator anim;
    private Rigidbody rb;

    //Slowmo variables
    private Vector3 snowballedForce;
    [SerializeField]
    private float multiplier;

    [SerializeField]
    private GameObject[] GrapplingMesh;

    private InputSystem_Actions inputs;

    private IMoveState[] moveStates;
    private IMoveState activeState;

    private bool canGrapple;
    private S_GrapplingMovement grapplingMovement;

    private bool isPaused;

    public Collider[] grapplingCollidersInRange { get; private set; }
    public Collider[] ziplinesInRange { get; private set; }
    public Collider[] movablesInRange { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputs = new InputSystem_Actions();
        moveStates = GetComponents<IMoveState>();
        activeState = moveStates[0];

        foreach (IMoveState moveState in moveStates)
        {
            var stateComponent = moveState as MonoBehaviour;
            if (moveState is S_GrapplingMovement)
            {
                if (stateComponent.enabled)
                {
                    canGrapple = true;
                    Debug.Log("Has Grapple ability");
                }
                else
                {
                    foreach (GameObject mesh in GrapplingMesh)
                    {
                        mesh.SetActive(false);
                    }
                }
                grapplingMovement = (S_GrapplingMovement)moveState;
            }
        }

        //anim = GetComponent<Animator>();
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
        inputs.Player.Skill.performed += Skill_performed;        
    }
    /// <summary>
    /// Slows time
    /// </summary>
    /// <param name="obj"></param>
    private void Skill_performed(InputAction.CallbackContext obj)
    {
        activeState.Skill_Perform(obj);
        /*originalTimeScale = Time.timeScale;
        Time.timeScale = slowedTimeScale;        
        currentSlowedTime = 0;
        snowballedForce = Vector3.zero;
        //anim.speed /= (slowedTimeScale/2);
        bSlowedTime = true;*/
    }

    private void Jump_canceled(InputAction.CallbackContext obj)
    {
        if (isPaused) return;
        activeState.Jump_Cancel(obj);
    }

    private void MoveCable_canceled(InputAction.CallbackContext obj)
    {
        if (isPaused) return;
        activeState.MoveCable_Cancel(obj);
    }

    private void MoveCable_performed(InputAction.CallbackContext obj)
    {
        if (isPaused) return;
        activeState.MoveCable_Perform(obj);
    }

    private void Attack_canceled(InputAction.CallbackContext obj)
    {
        if (isPaused) return;
        activeState.Attack_Cancel(obj);
    }

    private void Attack_performed(InputAction.CallbackContext obj)
    {
        if (isPaused) return;
        activeState.Attack_Perform(obj);
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        if (isPaused) return;
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
        inputs.Player.Skill.performed -= Skill_performed;
    }

    private void Move_canceled(InputAction.CallbackContext obj)
    {
        if (isPaused) return;
        activeState.Move_Cancel(obj);
    }

    private void Move_performed(InputAction.CallbackContext obj)
    {
        if (isPaused) return;
        activeState.Move_Perform(obj);
    }
    private void FixedUpdate()
    {
        if (isPaused) return;

        activeState.StateFixedUpdate();
        GrapplingRange();
        ZiplineRange();
        MovableObjectRange();
        BlobShadow();
    }
    // Update is called once per frame
    void Update()
    {
        if(isPaused) return;
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

    private void ZiplineRange()
    {
        Collider[] objs = Physics.OverlapSphere(transform.position, 5f, 1 << 12);
        if(objs != null)
        {
            ziplinesInRange = objs;
        }
    }

    private void MovableObjectRange()
    {
        Collider[] objs = Physics.OverlapSphere(transform.position, 6f, 1 << 8);
        if(objs != null)
        {
            movablesInRange = objs;
        }
    }
    public void ChangeState(Type state)
    {
        if (!typeof(IMoveState).IsAssignableFrom(state))
        {
            Debug.LogError("Invalid type passed to ChangeState. Must be a subclass of IMoveState.");
            return;
        }

        foreach (IMoveState s in moveStates)
        {
            if (s.GetType() == state)
            {                
                Debug.Log("Changed State to: "+ state.ToString());
                activeState = s;
                activeState.Activation();
                break;
            }
        }
    }

    public void EnableGrappling(bool newState)
    {
        canGrapple = newState;
        GetComponent<S_GrapplingMovement>().enabled = newState;
        foreach (GameObject mesh in GrapplingMesh)
        {
            mesh.SetActive(true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 100f);
    }

    public void PausePlayer(bool pausePlayerMovement = true, bool pausePlayerInput = true)
    {
        if (pausePlayerMovement)
            isPaused = true;
        else
            isPaused = false;

        if (pausePlayerInput)
            inputs.Disable();
        else
            inputs.Enable();
    }

    public bool GetPauseStatus()
    {
        return isPaused;
    }

    public void AddPlayerForce(Vector3 force, ForceMode forceMode = ForceMode.Force)
    {
        rb.AddForce(force, forceMode);

        if(bSlowedTime)        
        {
            snowballedForce += force*multiplier;
        }
    }

    public void ApplySnowballedForce()
    {
        rb.AddForce(snowballedForce, ForceMode.Force);
        snowballedForce = Vector3.zero;
    }
}
