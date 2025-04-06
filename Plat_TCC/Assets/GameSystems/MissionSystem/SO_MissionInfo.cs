using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionInfo", menuName = "Missions/MissionInfo")]
public class SO_MissionInfo : ScriptableObject
{
    [Header("Objects Ids"), Tooltip("Mission Exclusive Objects")]
    // Make this private with [SerializeField]
    public int[] missionObjectIds;

    public int[] missionCollectibleIds;

    [SerializeField]
    public GameObject objective;

    [Header("Mission Identification"), SerializeField]
    public int level;
    [SerializeField]
    public int misisonIndex;
}