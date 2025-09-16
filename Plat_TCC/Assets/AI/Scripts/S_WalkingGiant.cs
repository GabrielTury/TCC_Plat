using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class S_WalkingGiant : S_InteractableBase
{
    [SerializeField]
    private float detectionRadius;

    [SerializeField]
    private float distanceToWalk = 5f;

    [SerializeField]
    private LayerMask layerToAvoid = 1 << 8;

    private Collider[] objsInRange;

    private Animator anim;

    NavMeshAgent nav;
    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }
    protected override IEnumerator Idle_Exec()
    {
        anim.SetBool("IsWalking", false);
        yield return null;
    }

    protected override IEnumerator Interacting_Exec()
    {
        anim.SetBool("IsWalking", true);
        Vector3[] avoidPos = new Vector3[objsInRange.Length];

        for (int i = 0; i < objsInRange.Length; i++)
        {
            avoidPos[i] = (transform.position - objsInRange[i].transform.position); //gets a point in the oposite direction of the object
        }

        if(avoidPos.Length == 1)
            nav.destination = transform.position + avoidPos[0].normalized * distanceToWalk;
        else
        {
            Vector3 averagePos = Vector3.zero; //calculates average position to run from all points
            for (int i = 0; i < avoidPos.Length; i++)
            {
                averagePos += avoidPos[i];
            }
            averagePos /= avoidPos.Length;
            
            nav.destination = transform.position + averagePos.normalized * distanceToWalk;            
        }
        
        yield return null;
    }

    protected override IEnumerator Moving_Exec()
    {
        throw new System.NotImplementedException();
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        Collider[] temp = Physics.OverlapSphere(transform.position, detectionRadius, layerToAvoid, QueryTriggerInteraction.Collide);

        if (temp != objsInRange && temp.Length != 0)
        {
            objsInRange = temp;
            ChangeState(InteractableState.Interacting);
        }
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
