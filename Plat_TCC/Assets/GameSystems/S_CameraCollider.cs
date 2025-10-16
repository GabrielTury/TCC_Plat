using UnityEngine;

public class S_CameraCollider : MonoBehaviour
{
    [SerializeField]
    private GameObject cameraColliders;

    private void OnTriggerEnter(Collider other)
    {
        cameraColliders.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        cameraColliders.SetActive(false);
    }
}
