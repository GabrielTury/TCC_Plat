using UnityEngine;

[RequireComponent (typeof(Collider))]
public class S_JumpModifier : MonoBehaviour
{    
    private Collider jumpCollider;

    [SerializeField]
    private float force;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        jumpCollider = GetComponent<Collider>();
        jumpCollider.isTrigger = true;

    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 og = other.attachedRigidbody.linearVelocity;
        og.y = 0;
        other.attachedRigidbody.linearVelocity = og;
        other.attachedRigidbody.AddForce(Vector3.up * force, ForceMode.Impulse);
        Debug.Log("Jumped In Head");
    }
}
