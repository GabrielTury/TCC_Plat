using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class S_ChangeCamPerspective : MonoBehaviour
{
    [SerializeField]
    private CinemachineCamera changeCam;

    [SerializeField]
    private Transform lookAtTarget;

    [SerializeField]
    private Transform playerTransform;


    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        changeCam.enabled = false;
    }

    private void LateUpdate()
    {
        // Move our position a step closer to the target.
        var step = 100 * Time.deltaTime; // calculate distance to move

        Vector3 nextPos = lookAtTarget.position;
        nextPos.y = playerTransform.position.y;
        lookAtTarget.position = Vector3.MoveTowards(lookAtTarget.position, nextPos, step);
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
