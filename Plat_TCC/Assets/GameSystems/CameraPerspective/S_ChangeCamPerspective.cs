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

    [SerializeField]
    private Transform trackingTarget;

    [SerializeField]
    private bool trackTarget = true;
    private bool tracking;


    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        changeCam.enabled = false;

        if (trackingTarget != null)
            changeCam.Target = new CameraTarget { TrackingTarget = trackingTarget };
    }

    private void LateUpdate()
    {
        if (!tracking) return;
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

        if(trackTarget)
            tracking = true;
    }

    private void OnTriggerExit(Collider other)
    {        
        changeCam.enabled = false;
        changeCam.Priority = -1;

        if(trackTarget)
            tracking = false;
    }

}
