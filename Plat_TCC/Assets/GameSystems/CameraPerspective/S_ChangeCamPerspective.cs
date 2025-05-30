using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class S_ChangeCamPerspective : MonoBehaviour
{
    [SerializeField]
    private CinemachineCamera changeCam;

    private void Start()
    {
        changeCam.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        changeCam.enabled = true;
        changeCam.Priority = 1;
    }

    private void OnTriggerExit(Collider other)
    {        
        changeCam.enabled = false;
        changeCam.Priority = -1;
    }

}
