using UnityEngine;
using UnityEngine.InputSystem;

public interface IMoveState
{
    /// <summary>
    /// Needs to be called when Input "MoveCable".cancel is executed
    /// </summary>
    /// <param name="obj">Input System action callback value</param>
    public void MoveCable_Cancel(InputAction.CallbackContext obj);
    /// <summary>
    /// Needs to be called when Input "MoveCable".perform is executed
    /// </summary>
    /// <param name="obj">Input System action callback value</param>
    public void MoveCable_Perform(InputAction.CallbackContext obj);
    /// <summary>
    /// Needs to be called when Input "Move".perform is executed
    /// </summary>
    /// <param name="obj">Input System action callback value</param>
    public void Move_Perform(InputAction.CallbackContext obj);
    /// <summary>
    /// Needs to be called when Input "Move".cancel is executed
    /// </summary>
    /// <param name="obj">Input System action callback value</param>
    public void Move_Cancel(InputAction.CallbackContext obj);
    /// <summary>
    /// Needs to be called when Input "Jump".perform is executed
    /// </summary>
    /// <param name="obj">Input System action callback value</param>
    public void Jump_Perform(InputAction.CallbackContext obj);
    /// <summary>
    /// Needs to be called when Input "Jump".canceled is executed
    /// </summary>
    /// <param name="obj">Input System action callback value</param>
    public void Jump_Cancel(InputAction.CallbackContext obj);
    /// <summary>
    /// Needs to be called when Input "Attack".perform is executed
    /// </summary>
    /// <param name="obj">Input System action callback value</param>
    public void Attack_Perform(InputAction.CallbackContext obj);
    /// <summary>
    /// Needs to be called when Input "Attack".cancelled is executed
    /// </summary>
    /// <param name="obj">Input System action callback value</param>
    public void Attack_Cancel(InputAction.CallbackContext obj);
    /// <summary>
    /// Needs to be called in FixedUpdate
    /// </summary>
    public void StateFixedUpdate();
    /// <summary>
    /// Needs to be called in Update
    /// </summary>
    public void StateUpdate();
}
