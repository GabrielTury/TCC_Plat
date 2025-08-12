using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "WorldInfo", menuName = "Mission System/WorldInfo")]
public class SO_WorldInfo : ScriptableObject
{
    public int worldId;

    public string[] missionSceneNames;

    [Tooltip("Each light info assigned to the mission in the same index at Mission Scene Names")]
    public LevelLightInfo[] levelLightInfos;
}

[Serializable]
public struct LevelLightInfo
{
    public Color lightColor;

    public float intensity;

    public Quaternion directionalLightRotation;

    public Material skybox;
}
