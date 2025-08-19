using UnityEngine;
using UnityEngine.InputSystem;

public class S_ZiplineMovement : MonoBehaviour, IMoveState
{
    [Header("Detection"), Range(1, 30)]
    public float ziplineDetectionRadius = 5f;

    #region Interface Methods
    public void Activation()
    {
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
        throw new System.NotImplementedException();
    }

    public void StateUpdate()
    {
        throw new System.NotImplementedException();
    }
    #endregion //Interface Methods

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
