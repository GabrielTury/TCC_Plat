using UnityEngine;

public class S_DevShortcuts : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Clear All Missions")]
    public void ClearAllWorlds()
    {
        S_SaveManager.instance.SetMissionStatus(1, 1, true);
        S_SaveManager.instance.SetMissionStatus(1, 2, true);
        S_SaveManager.instance.SetMissionStatus(1, 3, true);
        S_SaveManager.instance.SetMissionStatus(2, 1, true);
        S_SaveManager.instance.SetMissionStatus(2, 2, true);
        S_SaveManager.instance.SetMissionStatus(2, 3, true);
        S_SaveManager.instance.SavePlayerData(S_SaveManager.instance.playerData);
    }

    [ContextMenu("Reset All Missions")]
    public void ResetAllWorlds()
    {
        S_SaveManager.instance.SetMissionStatus(1, 1, false);
        S_SaveManager.instance.SetMissionStatus(1, 2, false);
        S_SaveManager.instance.SetMissionStatus(1, 3, false);
        S_SaveManager.instance.SetMissionStatus(2, 1, false);
        S_SaveManager.instance.SetMissionStatus(2, 2, false);
        S_SaveManager.instance.SetMissionStatus(2, 3, false);
        S_SaveManager.instance.SavePlayerData(S_SaveManager.instance.playerData);
    }
}
