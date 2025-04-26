using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class S_LevelManager : MonoBehaviour
{
    public static S_LevelManager instance;

    public int collectibles = 0;
    public int mainCollectibles = 0;
    public Vector3 playerPositionCheckpoint;

    public GameObject[] savedObjects;
    public List<Transform> savedObjectsTransform = new List<Transform>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        savedObjectsTransform = new List<Transform>(savedObjects.Length);

        // Set Checkpoint Data to player initial position

        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        SetCheckpointData(playerPos);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCheckpointData(Vector3 checkpointPosition)
    {
        playerPositionCheckpoint = checkpointPosition;

        int index = 0;
        foreach (var obj in savedObjects)
        {
            try
            {
                savedObjectsTransform[index] = obj.transform;
            }
            catch
            {
                Debug.LogWarning("[Checkpoint Data] Object " + obj + " does not have a transform.");
            }
            index++;
        }
    }

    public void ResetLevel()
    {
        // Reset player position
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = playerPositionCheckpoint;
        }
        // Reset collectibles
        //collectibles = 0;
        // Reset saved objects
        for (int i = 0; i < savedObjects.Length; i++)
        {
            if (savedObjects[i] != null && savedObjectsTransform[i] != null)
            {
                savedObjects[i].transform.position = savedObjectsTransform[i].position;
                savedObjects[i].transform.rotation = savedObjectsTransform[i].rotation;
            }
        }
    }
}
