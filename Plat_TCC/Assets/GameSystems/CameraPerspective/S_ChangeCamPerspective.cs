using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class S_ChangeCamPerspective : MonoBehaviour
{
    private Camera cam;
    private CinemachineCamera cineCam;
    private CinemachineOrbitalFollow orbit;

    [SerializeField]
    private Transform target;

    private void Start()
    {
        cam = Camera.main;
        cineCam = FindFirstObjectByType<CinemachineCamera>();
        orbit = FindFirstObjectByType<CinemachineOrbitalFollow>();
    }

    private void OnTriggerEnter(Collider other)
    {
        cineCam.LookAt = target;
        orbit.Radius = 30f;
        other.GetComponent<CinemachineInputAxisController>().enabled = false;

    }

    private void OnTriggerExit(Collider other)
    {
        cineCam.LookAt = other.transform;
        orbit.Radius = 10f;
        other.GetComponent<CinemachineInputAxisController>().enabled = true;
    }

}
