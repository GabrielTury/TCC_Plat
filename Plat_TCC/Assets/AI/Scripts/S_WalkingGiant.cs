using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class S_WalkingGiant : S_InteractableBase
{
    [SerializeField]
    private float detectionRadius;

    private Collider[] objsInRange;

    NavMeshAgent nav;

    private Vector3 destination;
    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }
    protected override IEnumerator Idle_Exec()
    {
        yield return null;
    }

    protected override IEnumerator Interacting_Exec()
    {
        Vector3[] avoidPos = new Vector3[objsInRange.Length];

        for (int i = 0; i < objsInRange.Length; i++)
        {
            avoidPos[i] = (transform.position - objsInRange[i].transform.position) * 15f; //gets a point in the oposite direction of the object
        }

        if(avoidPos.Length == 1)
            nav.destination = avoidPos[0].normalized * 15f;
        else
        {
            Vector3 averagePos = Vector3.zero; //calculates average position to run from all points
            for (int i = 0; i < avoidPos.Length; i++)
            {
                averagePos += avoidPos[i];
            }
            averagePos /= avoidPos.Length;
            
            nav.destination = averagePos.normalized * 15f;            
        }
        
        yield return null;
    }

    protected override IEnumerator Moving_Exec()
    {
        throw new System.NotImplementedException();
    }

    private void Update()
    {
        Collider[] temp = Physics.OverlapSphere(transform.position, detectionRadius, 1<<8, QueryTriggerInteraction.Collide);

        if(temp != objsInRange && temp.Length != 0)
        {
            objsInRange = temp;
            ChangeState(InteractableState.Interacting);
        }
    }

    private void FixedUpdate()
    {

    }

    private void Start()
    {
        ChangeState(InteractableState.Idle);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

}
