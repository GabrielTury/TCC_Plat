using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class S_EnableComponent : MonoBehaviour
{

    [SerializeField] private AudioClip grappleCollected;

    private enum Component
    {
        GrapplingMovement,
        GroundMovement,
    }

    [SerializeField]
    private Component componentToEnable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<S_PlayerMovement>())
        {
            switch(componentToEnable)
            {
                case Component.GrapplingMovement:                    
                    other.GetComponentInParent<S_PlayerMovement>().EnableGrappling(true);
                    break;

                default:
                    break;
            }

            AudioManager.instance.PlaySFX(grappleCollected);
            Destroy(gameObject); //Destroy the object
        }
    }
}
