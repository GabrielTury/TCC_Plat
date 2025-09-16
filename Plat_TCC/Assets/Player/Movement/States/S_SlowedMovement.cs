using UnityEngine;
using UnityEngine.InputSystem;

public class S_SlowedMovement : MonoBehaviour, IMoveState
{
    private float originalTimeScale;
    [SerializeField]
    private float slowedTimeScale, slowDuration;

    private float currentSlowedTime;
    private Vector3 snowballedForce;
    private bool bSlowedTime;

    private S_PlayerMovement playerMovement;
    private S_GroundMovement groundMovement;
    public void Activation()
    {
        originalTimeScale = Time.timeScale;
        Time.timeScale = slowedTimeScale;
        currentSlowedTime = 0;
        snowballedForce = Vector3.zero;
        bSlowedTime = true;
        playerMovement.bSlowedTime = true;
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
        groundMovement.Jump_Cancel(obj);
    }

    public void Jump_Perform(InputAction.CallbackContext obj)
    {
        groundMovement.Jump_Perform(obj);
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
    }

    public void Move_Perform(InputAction.CallbackContext obj)
    {
        groundMovement.Move_Perform(obj);
    }

    public void Skill_Perform(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }

    public void StateFixedUpdate()
    {
        groundMovement.StateFixedUpdate();
    }

    public void StateUpdate()
    {
        if (bSlowedTime)
        {
            currentSlowedTime += Time.unscaledDeltaTime;
            if (currentSlowedTime >= slowDuration)
            {
                Time.timeScale = originalTimeScale;
                bSlowedTime = false;
                playerMovement.bSlowedTime = false;
                playerMovement.ApplySnowballedForce();
                playerMovement.ChangeState(typeof(S_GroundMovement));
                //anim.speed = 1;
            }
        }
    }

    private void Start()
    {
        playerMovement = GetComponent<S_PlayerMovement>();
        groundMovement = GetComponent<S_GroundMovement>();
    }
}
