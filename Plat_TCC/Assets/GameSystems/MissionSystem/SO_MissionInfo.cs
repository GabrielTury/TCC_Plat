using UnityEngine;

[CreateAssetMenu(fileName = "MissionInfo", menuName = "Missions/MissionInfo")]
public class SO_MissionInfo : ScriptableObject
{
    [Header("Objects"), Tooltip("Mission Exclusive Objects")]
    public GameObject[] missionObjects {  get; private set; }

    public GameObject[] missionCoins { get; private set; }

    public GameObject objective;
    [Header("Mission Identification")]
    public int level { get; private set; }

    public int levelIndex { get; private set; }

    public void SetCoins(GameObject[] input)
    {
        missionCoins = input;
    }

    public void SetMissionObjects(GameObject[] input)
    {
        missionObjects = input;
    }

    public void SetObjective(GameObject input)
    {
        objective = input;
    }

}
