using System.Collections.Generic;
using System;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class S_SaveManager : MonoBehaviour
{
    public static S_SaveManager instance;

    // Structs for Player Data and World Save Data
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

    [System.Serializable]
    public struct SettingsData
    {
        public SettingsData(int resolutionIndexTemp, int windowTypeIndexTemp, float musicVolumeTemp, float soundVolumeTemp)
        {
            resolutionIndex = resolutionIndexTemp;
            windowTypeIndex = windowTypeIndexTemp;
            musicVolume = musicVolumeTemp;
            soundVolume = soundVolumeTemp;
        }

        public int resolutionIndex;
        public int windowTypeIndex;
        public float musicVolume;
        public float soundVolume;
    }

    [System.Serializable]
    public struct WorldSave
    {
        public WorldSave(List<bool> status)
        {
            missionStatus = status;
        }
        [SerializeField]
        public List<bool> missionStatus;
    }

    public PlayerData defaultData;

    public SettingsData defaultSettings;

    public WorldSave defaultWorld;

    public PlayerData playerData;

    public SettingsData settingsData;

    public void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        // Default Data Setup
        List<bool> defaultValues= new List<bool>();
        for (int i = 0; i < 4; i++)
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

        // Load Player Data
        (loadedPlayerData, worked) = LoadPlayerData();

        // If loading worked, use loaded data, otherwise use default data
        if (worked)
        {
            playerData = loadedPlayerData;
        }
        else
        {
            playerData = defaultData;
            SavePlayerData(playerData);
        }

        defaultSettings = new SettingsData(0, 0, 1.0f, 1.0f);

        SettingsData loadedSettingsData;
        (loadedSettingsData, worked) = LoadSettingsData();

        if (worked)
        {
            settingsData = loadedSettingsData;
        }
        else
        {
            settingsData = defaultSettings;
            SaveSettingsData(settingsData);
        }

        GetPlayerDataDebugInfo();
        GetSettingsDataDebugInfo();
    }

    /// <summary>
    /// Returns the status of a mission in a world. True if completed, false if not completed.
    /// </summary>
    /// <param name="worldNum"></param>
    /// <param name="missionNum"></param>
    /// <returns></returns>
    public bool GetMissionStatus(int worldNum, int missionNum)
    {
        return playerData.worlds[worldNum].missionStatus[missionNum];
    }

    /// <summary>
    /// Sets the status of a mission in a world. True if completed, false if not completed.
    /// </summary>
    /// <param name="worldNum"></param>
    /// <param name="missionNum"></param>
    /// <param name="status"></param>
    public void SetMissionStatus(int worldNum, int missionNum, bool status)
    {
        // Defensive: Ensure worlds and missionStatus are initialized
        if (playerData.worlds == null || playerData.worlds.Count <= worldNum)
        {
            Debug.LogWarning($"playerData.worlds was null or did not have enough worlds (requested: {worldNum}, actual: {(playerData.worlds == null ? "null" : playerData.worlds.Count.ToString())}). Initializing worlds.");
            // Initialize worlds with default values if null or not enough worlds
            List<WorldSave> tempWorlds = playerData.worlds ?? new List<WorldSave>();
            while (tempWorlds.Count <= worldNum)
            {
                List<bool> defaultMissions = new List<bool> { false, false, false };
                tempWorlds.Add(new WorldSave(defaultMissions));
            }
            playerData = new PlayerData(playerData.appleCount, tempWorlds);
            Debug.Log($"playerData.worlds initialized. New count: {playerData.worlds.Count}");
        }
        if (playerData.worlds[worldNum].missionStatus == null || playerData.worlds[worldNum].missionStatus.Count <= missionNum)
        {
            Debug.LogWarning($"playerData.worlds[{worldNum}].missionStatus was null or did not have enough missions (requested: {missionNum}, actual: {(playerData.worlds[worldNum].missionStatus == null ? "null" : playerData.worlds[worldNum].missionStatus.Count.ToString())}). Initializing missionStatus.");
            // Initialize missionStatus with default values if null or not enough missions
            List<bool> tempStatus = playerData.worlds[worldNum].missionStatus ?? new List<bool>();
            while (tempStatus.Count <= missionNum)
            {
                tempStatus.Add(false);
            }
            WorldSave tempWorld = new WorldSave(tempStatus);
            List<WorldSave> tempWorlds = new List<WorldSave>(playerData.worlds);
            tempWorlds[worldNum] = tempWorld;
            playerData = new PlayerData(playerData.appleCount, tempWorlds);
            Debug.Log($"playerData.worlds[{worldNum}].missionStatus initialized. New count: {playerData.worlds[worldNum].missionStatus.Count}");
        }

        // Create a copy to avoid modifying the original list reference
        List<bool> updatedStatus = new List<bool>(playerData.worlds[worldNum].missionStatus);
        updatedStatus[missionNum] = status;
        Debug.Log($"SetMissionStatus: worldNum={worldNum}, missionNum={missionNum}, status={status}");
        WorldSave updatedWorld = new WorldSave(updatedStatus);

        // Create a copy of worlds to avoid modifying the original list reference
        List<WorldSave> updatedWorlds = new List<WorldSave>(playerData.worlds);
        updatedWorlds[worldNum] = updatedWorld;
        playerData = new PlayerData(playerData.appleCount, updatedWorlds);
        Debug.Log($"playerData updated for worldNum={worldNum}, missionNum={missionNum}.");

        SavePlayerData(playerData);
    }

    /// <summary>
    /// Returns the current apple count of the player.
    /// </summary>
    /// <returns></returns>
    public int GetAppleCount()
    {
        return playerData.appleCount;
    }

    /// <summary>
    /// Sets the current apple count of the player.
    /// </summary>
    /// <param name="newCount"></param>
    public void SetAppleCount(int newCount)
    {
        playerData = new PlayerData(newCount, playerData.worlds);
    }

    /// <summary>
    /// Returns information about a world's save data.
    /// </summary>
    /// <param name="worldNum"></param>
    public void GetWorldSaveInfo(int worldNum)
    {
        if (worldNum == 0) { Debug.LogError("worldNum 0 is not valid"); return; }
    }

    /// <summary>
    /// Resets the player data to default values and saves it to file.
    /// </summary>
    public void ResetPlayerData()
    {
        playerData = new PlayerData(0, new List<WorldSave>());
        SavePlayerData(playerData);
    }

    /// <summary>
    /// Prints the current player data to the console for debugging purposes.
    /// </summary>
    public void GetPlayerDataDebugInfo()
    {
        string info = $"Apple Count: {playerData.appleCount}\nWorlds:";
        for (int i = 0; i < playerData.worlds.Count; i++)
        {
            var world = playerData.worlds[i];
            info += $"\n  World {i}:";
            if (world.missionStatus != null)
            {
                for (int j = 0; j < world.missionStatus.Count; j++)
                {
                    info += $"\n    Mission {j}: {(world.missionStatus[j] ? "Completed" : "Not Completed")}";
                }
            }
            else
            {
                info += "\n    No mission status data.";
            }
        }
        Debug.Log(info);
    }

    /// <summary>
    /// Prints the current settings data to the console for debugging purposes.
    /// </summary>
    public void GetSettingsDataDebugInfo()
    { 
        string info = $"Resolution Index: {settingsData.resolutionIndex}\nWindow Type Index: {settingsData.windowTypeIndex}\nMusic Volume: {settingsData.musicVolume}\nSound Volume: {settingsData.soundVolume}";
        Debug.Log(info);
    }

    /// <summary>
    /// Saves the current player data to a file in JSON format.
    /// </summary>
    /// <param name="playerData"></param>
    public void SavePlayerData(PlayerData playerData)
    {
        string jsonString = JsonUtility.ToJson(playerData, true);
        string path = Application.persistentDataPath + "/playerdata.playerdata";
        File.WriteAllText(path, jsonString);
    }

    /// <summary>
    /// Loads the player data from a file in JSON format. If the file does not exist, returns default player data.
    /// </summary>
    /// <returns></returns>
    public (PlayerData, bool) LoadPlayerData()
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

    /// <summary>
    /// Saves the current settings data to a file in JSON format.
    /// </summary>
    /// <param name="settingsData"></param>
    public void SaveSettingsData(SettingsData settingsData)
    {
        string jsonString = JsonUtility.ToJson(settingsData, true);
        string path = Application.persistentDataPath + "/settingsdata.settingsdata";
        File.WriteAllText(path, jsonString);
    }

    /// <summary>
    /// Loads the settings data from a file in JSON format. If the file does not exist, returns default settings data.
    /// </summary>
    /// <returns></returns>
    public (SettingsData, bool) LoadSettingsData()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        bool worked = true;
        try
        {
            FileInfo info = dir.GetFiles("settingsdata.settingsdata")[0];
            string fileContents = File.ReadAllText(info.FullName);
            var settingsData = JsonUtility.FromJson<SettingsData>(fileContents);
            return (settingsData, worked);
        }
        catch
        {
            worked = false;
            return (new SettingsData(), worked);
        }
    }
}
