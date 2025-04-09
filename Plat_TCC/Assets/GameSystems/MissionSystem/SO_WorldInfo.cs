using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "WorldInfo", menuName = "Mission System/WorldInfo")]
public class SO_WorldInfo : ScriptableObject
{
    public int worldId;

    public string[] missionSceneNames;
}
