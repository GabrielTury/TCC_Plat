using UnityEngine;
[RequireComponent(typeof(Collider))]
public class S_EnableComponent : MonoBehaviour
{

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
        if (other.CompareTag("Player"))
        {
            switch(componentToEnable)
            {
                case Component.GrapplingMovement:                    
                    other.GetComponentInParent<S_PlayerMovement>().EnableGrappling(true);
                    break;

                default:
                    break;
            }
                

            Destroy(gameObject); //Destroy the object
        }
    }
}
