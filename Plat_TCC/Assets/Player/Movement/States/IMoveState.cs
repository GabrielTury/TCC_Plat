using UnityEngine;
using UnityEngine.InputSystem;

public interface IMoveState
{    
    public void Move_Perform(InputAction.CallbackContext obj);

    public void Move_Cancel(InputAction.CallbackContext obj);

    public void Jump_Perform(InputAction.CallbackContext obj);

    public void Attack_Perform(InputAction.CallbackContext obj);

    public void MoveFixedUpdate();

    public void JumpFixedUpdate();

    public void AttackFixedUpdate();

    public void MoveUpdate();
}
