using System.Collections.Generic;
using System;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class S_SaveManager : MonoBehaviour
{
    [System.Serializable]
    public struct PlayerData
    {
        public PlayerData(int appleCountTemp, List<WorldSave> worldsTemp)
        {
            appleCount = appleCountTemp;
            worlds = worldsTemp;
        }
        public int appleCount;
        public List<WorldSave> worlds;
    }

    public struct WorldSave
    {
        public WorldSave(List<bool> status)
        {
            missionStatus = status;
        }
        public List<bool> missionStatus;
    }

    public PlayerData defaultData;

    public WorldSave defaultWorld;

    public static PlayerData playerData;

    public void Start()
    {
        List<bool> defaultValues= new List<bool>();
        for (int i = 0; i < 3; i++)
        {
            defaultValues.Add(false);
        }

        defaultWorld = new WorldSave(defaultValues);

        int appleCountTemp = 0;
        List<WorldSave> worldsTemp = new List<WorldSave>();
        for (int i = 0; i < 3; i++)
        {
            worldsTemp.Add(defaultWorld);
        }

        defaultData = new PlayerData(appleCountTemp, worldsTemp);

        bool worked;
        PlayerData loadedPlayerData;
        (loadedPlayerData, worked) = LoadPlayerData();

        if (worked)
        {
            playerData = loadedPlayerData;
        }
        else
        {
            playerData = defaultData;
        }

        Debug.Log(playerData.ToString());
    }

    public static bool GetMissionStatus(int worldNum, int missionNum)
    {
        return playerData.worlds[worldNum].missionStatus[missionNum];
    }
    
    public static void GetWorldSaveInfo(int worldNum)
    {
        if (worldNum == 0) { Debug.LogError("worldNum 0 is not valid"); return; }
    }

    public static void SavePlayerData(PlayerData playerData)
    {
        string jsonString = JsonUtility.ToJson(playerData, true);
        string path = Application.persistentDataPath + "/playerdata.playerdata";
        File.WriteAllText(path, jsonString);
    }

    public static (PlayerData, bool) LoadPlayerData()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        bool worked = true;
        try
        {
            FileInfo info = dir.GetFiles("playerdata.playerdata")[0];
            string fileContents = File.ReadAllText(info.FullName);
            var userData = JsonUtility.FromJson<PlayerData>(fileContents);
            return (userData, worked);
        }
        catch
        {
            worked = false;
            return (new PlayerData(), worked);
        }
    }
}
