using UnityEngine;

[CreateAssetMenu(fileName = "MissionUIInfo", menuName = "Mission System/MissionUIInfo")]
public class SO_MissionUIInfo : ScriptableObject
{
    [TextArea]
    public string objectiveName;
    public string condition;

    // a class with collectible icon, collectible name and count
    [System.Serializable]
    public class CollectibleInfo
    {
        public Sprite icon;
        public string collectibleName;
        public int count;
    }

    public CollectibleInfo[] collectibleInfo;

    // Variable to hold the time limit in seconds (0 means no time limit)

    public int timeLimitInSeconds;
}
