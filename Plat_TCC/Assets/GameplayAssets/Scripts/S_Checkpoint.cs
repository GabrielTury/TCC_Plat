using System.Collections;
using UnityEngine;
using HF = S_HelperFunctions;

public class S_Checkpoint : MonoBehaviour
{
    private bool playerIsInRegion = false;
    public bool hasActivatedCheckpoint = false;
    private Transform childPole;

    public Animator animator;

    [Header("Audio")]
    [SerializeField] private AudioClip checkpointSound;

    void Start()
    {
        childPole = transform.GetChild(0);
        animator = childPole.GetComponent<Animator>();
    }

    void Update()
    {
        if (playerIsInRegion && !hasActivatedCheckpoint)
        {
            hasActivatedCheckpoint = true;

            Vector3 playerPos = transform.position + new Vector3(0, 1.5f, 0);

            S_LevelManager.instance.SetCheckpointData(playerPos, this);

            // RaisePole ();
            animator.SetBool("IsActive", true);

            // Som de ativação do checkpoint
            AudioManager.instance.PlaySFX(checkpointSound);
        }
    }

    private void RaisePole()
    {
        Vector3 pos = transform.position + new Vector3(0, 1.5f, 0);
        StartCoroutine(HF.SmoothMoveTransform(childPole, pos, 1));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.CompareTag("Player"))
        {
            playerIsInRegion = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.CompareTag("Player"))
        {
            playerIsInRegion = false;
        }
    }
}
