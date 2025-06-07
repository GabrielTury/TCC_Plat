using System.Collections;
using UnityEngine;
using HF = S_HelperFunctions;

public class S_Checkpoint : MonoBehaviour
{
    private bool playerIsInRegion = false;
    private bool hasActivatedCheckpoint = false;
    private Transform childPole;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        childPole = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerIsInRegion && !hasActivatedCheckpoint)
        {
            hasActivatedCheckpoint = true;

            Vector3 playerPos = transform.position + new Vector3(0, 1.5f, 0);

            S_LevelManager.instance.SetCheckpointData(playerPos);

            RaisePole();
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
