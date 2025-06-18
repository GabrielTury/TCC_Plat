using System.Collections.Generic;
using UnityEngine;

public class S_LevelManager : MonoBehaviour
{
    public static S_LevelManager instance;

    public int collectibles = 0;
    public int mainCollectibles = 0;
    public int keyCollectibles = 0;
    public Vector3 playerPositionCheckpoint;

    public GameObject[] savedObjects;
    public List<Transform> savedObjectsTransform = new List<Transform>();

    private Transform playerTransform;

    private S_PlayerInformation playerInfo;

    private bool isResetting = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        savedObjectsTransform = new List<Transform>(savedObjects.Length);

        // Set Checkpoint Data to player initial position

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerInfo = playerTransform.GetComponent<S_PlayerInformation>();

        SetCheckpointData(playerTransform.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isResetting)
        {
            if (playerTransform.position != playerPositionCheckpoint)
            {
                playerTransform.position = playerPositionCheckpoint;
                isResetting = false;
                //Debug.LogWarning("[Checkpoint Update Data] Player position reset to checkpoint.");
            }
        }
    }

    public void AddCollectible(string collectibleName, int count)
    {
        switch (collectibleName)
        {
            case "Main":
                mainCollectibles += count;
                S_CollectibleExhibitor.instance.UpdateCollectible(collectibleName, mainCollectibles);
                break;

            case "Key":
                keyCollectibles += count;
                S_CollectibleExhibitor.instance.UpdateCollectible(collectibleName, keyCollectibles);
                break;

            case "Apple":
                collectibles += count;
                S_CollectibleExhibitor.instance.UpdateCollectible(collectibleName, collectibles);
                break;

            default:
                Debug.LogWarning("[Checkpoint Data] Collectible " + collectibleName + " not recognized.");
                break;
        }


        playerInfo.applesCollected = collectibles;
        //Debug.LogWarning("[Checkpoint Data] Collectible " + collectibleName + " collected. Total: " + collectibles);
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
            isResetting = true;
            //Debug.LogWarning("[Checkpoint Data] Player position reset to checkpoint.");
        } else
        {
            Debug.LogWarning("[Checkpoint Data] Player not found. Cannot reset position.");
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
