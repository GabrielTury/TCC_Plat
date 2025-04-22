using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class S_LevelManager : MonoBehaviour
{
    public static S_LevelManager instance;

    public int collectibles = 0;
    public Vector3 playerPositionCheckpoint;

    public GameObject[] savedObjects;
    public List<Transform> savedObjectsTransform = new List<Transform>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        savedObjectsTransform = new List<Transform>(savedObjects.Length);
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
}
