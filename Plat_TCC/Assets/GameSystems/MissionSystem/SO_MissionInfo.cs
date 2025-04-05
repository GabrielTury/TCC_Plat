using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionInfo", menuName = "Missions/MissionInfo")]
public class SO_MissionInfo : ScriptableObject
{
    [Header("Objects"), Tooltip("Mission Exclusive Objects")]
    // Make this private with [SerializeField]
    [SerializeField] private Object[] missionObjectReferences;

    // Create a property just like you did for coins
    public GameObject[] missionObjects
    {
        get
        {
            if (missionObjectReferences == null) return new GameObject[0];
            return missionObjectReferences.Select(o => o as GameObject).Where(g => g != null).ToArray();
        }
        set
        {
            if (value == null)
            {
                missionObjectReferences = new Object[0];
                return;
            }
            missionObjectReferences = value.Cast<Object>().ToArray();
        }
    }

    [SerializeField] private Object[] coinObjectReferences;
    public GameObject[] missionCoins
    {
        get
        {
            if (coinObjectReferences == null) return new GameObject[0];
            return coinObjectReferences.Select(o => o as GameObject).Where(g => g != null).ToArray();
        }
        set
        {
            if (value == null)
            {
                coinObjectReferences = new Object[0];
                return;
            }
            coinObjectReferences = value.Cast<Object>().ToArray();
        }
    }

    [SerializeField]
    public GameObject objective;

    [Header("Mission Identification"), SerializeField]
    public int level;
    [SerializeField]
    public int levelIndex;
}