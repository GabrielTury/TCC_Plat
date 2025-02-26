using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.InputSystem;

public class S_PlayerMovement : MonoBehaviour
{
    private InputSystem_Actions inputs;


    private IMoveState[] moveStates;
    private IMoveState activeState;

    private void Awake()
    {
        inputs = new InputSystem_Actions();
        moveStates = GetComponents<IMoveState>();
        activeState = moveStates[0];
    }
    private void OnEnable()
    {
        inputs.Enable();
        inputs.Player.Move.performed += Move_performed;
        inputs.Player.Move.canceled += Move_canceled;
        inputs.Player.Jump.performed += Jump_performed;
        inputs.Player.Attack.performed += Attack_performed;
    }

    private void Attack_performed(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
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
        inputs.Player.Attack.performed -= Attack_performed;
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
    }
    // Update is called once per frame
    void Update()
    {
        activeState.StateUpdate();
    }

}
