using UnityEngine;
[RequireComponent(typeof(Collider))]
public class S_PlatformMovement : MonoBehaviour
{
    private Transform moveObject;

    private Vector3 lastPosition;


    private void Start()
    {
        lastPosition = transform.position;
    }
    private void FixedUpdate()
    {
        Vector3 resultMovement = transform.position - lastPosition;

        if (moveObject != null)
        {
            moveObject.position += resultMovement;
        }

        lastPosition = transform.position;
    }
}
