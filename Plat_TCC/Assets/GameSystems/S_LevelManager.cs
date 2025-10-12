using System.Collections.Generic;
using UnityEngine;

public class S_LevelManager : MonoBehaviour
{
    public static S_LevelManager instance;
    public S_PlayerMovement playerMovement {  get; private set; }

    public int collectibles = 0;
    public int mainCollectibles = 0;
    /// <summary>
    /// Keys collected
    /// </summary>
    public int keyCollectibles = 0;
    public Vector3 playerPositionCheckpoint;

    public GameObject[] savedObjects;
    public List<Transform> savedObjectsTransform = new List<Transform>();

    private S_Checkpoint[] checkpoints;

    public Transform playerTransform;

    private S_PlayerInformation playerInfo;

    private bool isResetting = false;

    public System.Action<int> OnAppleCollected;

    public System.Action<int> OnKeyCollected;

    public System.Action<int> OnGearCollected;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        savedObjectsTransform = new List<Transform>(savedObjects.Length);

        // Set Checkpoint Data to player initial position

        playerTransform = GameObject.Find("Player").transform;
        playerInfo = playerTransform.GetComponent<S_PlayerInformation>();
        playerMovement = playerTransform.GetComponent<S_PlayerMovement>();

        SetCheckpointData(playerTransform.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isResetting)
        {
            if (playerTransform == null)
            {
                playerTransform = GameObject.Find("Player").transform;
            }
            if (playerTransform.position != playerPositionCheckpoint)
            {
                playerTransform.position = playerPositionCheckpoint;
                isResetting = false;
                Debug.LogWarning("[Checkpoint Update Data] Player position trying to reset to checkpoint.");
            }
        }
    }

    public void PerformOnSceneLoad()
    {
        // Find all objects that contain S_Checkpoint script in the scene
        checkpoints = FindObjectsByType<S_Checkpoint>(FindObjectsSortMode.InstanceID);
        Debug.Log("[Checkpoint Data] Found " + checkpoints.Length + " checkpoints in the scene.");
    }

    // ...

    public void AddCollectible(string collectibleName, int count)
    {
        switch (collectibleName)
        {
            case "Main":
                mainCollectibles += count;
                S_CollectibleExhibitor.instance.UpdateCollectible(collectibleName, mainCollectibles);
                OnGearCollected?.Invoke(count);
                break;

            case "Key":
                keyCollectibles += count;
                S_CollectibleExhibitor.instance.UpdateCollectible(collectibleName, keyCollectibles);
                OnKeyCollected?.Invoke(count);
                break;

            case "Apple":
                collectibles += count;
                S_CollectibleExhibitor.instance.UpdateCollectible(collectibleName, collectibles);
                OnAppleCollected?.Invoke(count);
                break;

            default:
                Debug.LogWarning("[Checkpoint Data] Collectible " + collectibleName + " not recognized.");
                break;
        }

        playerInfo.applesCollected = collectibles;
        //Debug.LogWarning("[Checkpoint Data] Collectible " + collectibleName + " collected. Total: " + collectibles);
    }

    public void SetCheckpointData(Vector3 checkpointPosition, S_Checkpoint checkpoint = null)
    {
        playerPositionCheckpoint = checkpointPosition;
        Debug.Log("[Checkpoint Data] Set Player Position Checkpoint: " + checkpointPosition);

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

        if (checkpoint != null)
        {
            // For each checkpoint that is not the current one, reset its state
            foreach (S_Checkpoint ck in checkpoints)
            {
                if (ck != checkpoint)
                {
                    ck.animator.SetBool("IsActive", false);
                    ck.hasActivatedCheckpoint = false;
                    Debug.Log(ck.name + " deactivated.");
                }
            }

            Debug.Log("[Checkpoint Data] Checkpoint set by: " + checkpoint.name);
        }
    }

    public void ResetLevel()
    {
        // Reset player position

        //GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (playerTransform == null)
        {
            playerTransform = GameObject.Find("Player").transform;
        }
        if (playerTransform != null)
        {
            isResetting = true;
            playerTransform.position = playerPositionCheckpoint;
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
