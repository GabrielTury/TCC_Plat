using UnityEngine;

[RequireComponent (typeof(Collider))]
public class S_JumpModifier : MonoBehaviour
{    
    private Collider jumpCollider;

    [SerializeField]
    private float force;

    private IDamageable parentDamage;

    //[SerializeField]
    //private 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        jumpCollider = GetComponent<Collider>();
        jumpCollider.isTrigger = true;

    }

    private void Start()
    {
        parentDamage = GetComponentInParent<IDamageable>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 og = other.attachedRigidbody.linearVelocity;
        og.y = 0;
        other.attachedRigidbody.linearVelocity = og;
        other.attachedRigidbody.AddForce(Vector3.up * force, ForceMode.Impulse);
        //Debug.Log("Jumped In Head");

        if(parentDamage != null)
        {
            bool dead = parentDamage.TakeDamage(1, "Hit");
            if (dead)
            {
                jumpCollider.enabled = false;
            }
        }
        else
        {
            Debug.LogWarning("Parent does not implement IDamageable interface.");
        }
    }
}
