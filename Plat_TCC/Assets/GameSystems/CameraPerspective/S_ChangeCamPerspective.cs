using Unity.Cinemachine;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

[RequireComponent(typeof(Collider))]
public class S_ChangeCamPerspective : MonoBehaviour
{
    private CinemachineCamera cineCam;
    private CinemachineOrbitalFollow orbit;
    private CinemachineRotationComposer rot;


    #region Original Variables
    private bool ogWrap;
    private Vector2 ogRange;
    private Rect originalDeadZone;
    #endregion
    [SerializeField]
    private Transform target;

    private void Start()
    {
        cineCam = FindFirstObjectByType<CinemachineCamera>();
        orbit = FindFirstObjectByType<CinemachineOrbitalFollow>();
        rot = FindFirstObjectByType<CinemachineRotationComposer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        cineCam.Target.TrackingTarget = target;
        orbit.Radius = 30f;
        ogWrap = orbit.HorizontalAxis.Wrap;
        ogRange = orbit.HorizontalAxis.Range;
        orbit.HorizontalAxis.Wrap = false;
        orbit.HorizontalAxis.Range = new Vector2(0,10);


        originalDeadZone = rot.Composition.DeadZoneRect;
        rot.Composition.DeadZoneRect = new Rect(0.5f, 0.5f, 0, 0);

        other.GetComponent<CinemachineInputAxisController>().enabled = false;

    }

    private void OnTriggerExit(Collider other)
    {
        cineCam.Target.TrackingTarget = other.transform;
        orbit.Radius = 10f;
        orbit.HorizontalAxis.Wrap = ogWrap;
        orbit.HorizontalAxis.Range = ogRange;
        rot.Composition.DeadZoneRect = originalDeadZone;

        other.GetComponent<CinemachineInputAxisController>().enabled = true;
    }

}
