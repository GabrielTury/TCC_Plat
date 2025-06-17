using UnityEngine;

[CreateAssetMenu(fileName = "MissionUIInfo", menuName = "Mission System/MissionUIInfo")]
public class SO_MissionUIInfo : ScriptableObject
{
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

}
