using UnityEngine;

public class TEST_MissionCaller : MonoBehaviour
{
    [ContextMenu("LoadLevel")]
    void LoadLevel()
    {
        GetComponent<S_MissionManager>().StartMission(Callback, 1);
    }

    private void Callback(bool result)
    {
        if (result)
        {
            Debug.Log("Level loaded successfully");
        }
        else
        {
            Debug.Log("Failed to load level");
        }
    }
}
