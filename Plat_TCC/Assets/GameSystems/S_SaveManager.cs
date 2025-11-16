using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        public SettingsData(int resolutionIndexTemp, int windowTypeIndexTemp, bool vsyncEnabledTemp, int musicVolumeTemp, int soundVolumeTemp, string languageName)
        {
            resolutionIndex = resolutionIndexTemp;
            windowTypeIndex = windowTypeIndexTemp;
            vsyncEnabled = vsyncEnabledTemp;
            musicVolume = musicVolumeTemp;
            soundVolume = soundVolumeTemp;
            language = languageName;
        }

        public int resolutionIndex;
        public int windowTypeIndex;
        public bool vsyncEnabled;
        public int musicVolume;
        public int soundVolume;
        public string language;
    }

    [System.Serializable]
    public struct WorldSave
    {
        public WorldSave(List<bool> status, List<int> fruitCount, List<bool> timeChallenge)
        {
            missionStatus = status;
            fruitRecord = fruitCount;
            timeChallengeStatus = timeChallenge;
        }
        [SerializeField]
        public List<bool> missionStatus;

        [SerializeField]
        public List<int> fruitRecord;

        [SerializeField]
        public List<bool> timeChallengeStatus;
    }

    private List<(int width, int height)> resolutions = new List<(int width, int height)>
    {
        (1280, 720),
        (1366, 768),
        (1600, 900),
        (1920, 1080),
        (2560, 1440),
        (3840, 2160)
    };

    public PlayerData defaultData;

    public SettingsData defaultSettings;

    public WorldSave defaultWorld;

    public PlayerData playerData;

    public SettingsData settingsData;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(gameObject);

        defaultWorld = new WorldSave(new List<bool> { false, false, false }, new List<int> { 0, 0, 0 }, new List<bool> { false, false, false });

        defaultData = new PlayerData(0, new List<WorldSave> { defaultWorld, defaultWorld, defaultWorld});

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

        int maxResolutionIndex = resolutions.FindLastIndex(res => res.width <= Screen.currentResolution.width);

        // print all available resolutions and if they are less than or equal to the current resolution
        if (maxResolutionIndex > 0)
        {
            Debug.LogWarning("Available Resolutions:");
            for (int i = 0; i < resolutions.Count; i++)
            {
                var res = resolutions[i];
                Debug.LogWarning($"{i}: {res.width}x{res.height} {(res.width <= Screen.currentResolution.width ? "(Supported)" : "(Not Supported)")}");
            }
        }

        defaultSettings = new SettingsData(maxResolutionIndex, 1, false, 80, 80, "en");

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

        SceneManager.activeSceneChanged += OnSceneChanged;
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

    public int GetAppleRecord(int worldNum, int missionNum)
    {
        return playerData.worlds[worldNum].fruitRecord[missionNum];
    }

    /// <summary>
    /// Sets the status of a mission in a world. True if completed, false if not completed.
    /// </summary>
    /// <param name="worldNum"></param>
    /// <param name="missionNum"></param>
    /// <param name="status"></param>
    public void SetMissionStatus(int worldNum, int missionNum, bool status, bool timeChallenge)
    {
        worldNum = worldNum - 1;
        missionNum = missionNum - 1;
        // Defensive: Ensure worlds and missionStatus are initialized
        if (playerData.worlds == null || playerData.worlds.Count <= worldNum)
        {
            Debug.LogWarning($"playerData.worlds was null or did not have enough worlds (requested: {worldNum}, actual: {(playerData.worlds == null ? "null" : playerData.worlds.Count.ToString())}). Initializing worlds.");
            // Initialize worlds with default values if null or not enough worlds
            List<WorldSave> tempWorlds = playerData.worlds ?? new List<WorldSave>();
            while (tempWorlds.Count <= worldNum)
            {
                List<bool> defaultMissions = new List<bool> { false, false, false };

                List<int> defaultFruitRecord = new List<int> { 0, 0, 0 };

                List<bool> defaultTimeChallenge = new List<bool> { false, false, false };

                tempWorlds.Add(new WorldSave(defaultMissions, defaultFruitRecord, defaultTimeChallenge));
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
            WorldSave tempWorld = new WorldSave(tempStatus, new List<int> { 0, 0, 0 }, tempStatus);
            List<WorldSave> tempWorlds = new List<WorldSave>(playerData.worlds);
            tempWorlds[worldNum] = tempWorld;
            playerData = new PlayerData(playerData.appleCount, tempWorlds);
            Debug.Log($"playerData.worlds[{worldNum}].missionStatus initialized. New count: {playerData.worlds[worldNum].missionStatus.Count}");
        }

        // Create a copy to avoid modifying the original list reference
        List<bool> updatedStatus = new List<bool>(playerData.worlds[worldNum].missionStatus);
        updatedStatus[missionNum] = status;
        Debug.Log($"SetMissionStatus: worldNum={worldNum}, missionNum={missionNum}, status={status}");

        // Correcting the type mismatch by accessing the correct element from the fruitRecord list
        int lastAppleRecord = playerData.worlds[worldNum].fruitRecord[missionNum];

        if (S_LevelManager.instance != null)
        {
            if (S_LevelManager.instance.collectibles > lastAppleRecord)
            {
                lastAppleRecord = S_LevelManager.instance.collectibles;
            }
        }

        // Correcting the type mismatch by providing a List<int> instead of an int
        List<int> updatedFruitRecord = new List<int>(playerData.worlds[worldNum].fruitRecord);
        updatedFruitRecord[missionNum] = lastAppleRecord;

        // Timed challenge status update
        List<bool> updatedTimeChallenge = new List<bool>(playerData.worlds[worldNum].timeChallengeStatus);
        updatedTimeChallenge[missionNum] = timeChallenge;

        WorldSave updatedWorld = new WorldSave(updatedStatus, updatedFruitRecord, updatedTimeChallenge);
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

    public bool GetTimeChallengeStatus(int worldNum, int missionNum)
    {
        return playerData.worlds[worldNum].timeChallengeStatus[missionNum];
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
    /// Resets the player data to default values and saves it.
    /// </summary>
    public void ResetPlayerData()
    {
        playerData = defaultData;
        SavePlayerData(playerData);
    }

    /// <summary>
    /// Saves the current player data to a file in JSON format.
    /// </summary>
    /// <param name="playerData"></param>
    public void SavePlayerData(PlayerData playerData)
    {
        string jsonString = JsonUtility.ToJson(playerData, true);
        string path = Application.persistentDataPath + "/playerdatanew.playerdata";
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
            FileInfo info = dir.GetFiles("playerdatanew.playerdata")[0];
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
        string path = Application.persistentDataPath + "/settingsdatanew.settingsdata";
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
            FileInfo info = dir.GetFiles("settingsdatanew.settingsdata")[0];
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

    public SettingsData GetSettingsData()
    {
        return settingsData;
    }

    public PlayerData GetPlayerData()
    {
        return playerData;
    }

    private void OnSceneChanged(Scene current, Scene next)
    {
        SettingsData loadedSettingsData;
        bool worked;

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

        PlayerData loadedPlayerData;

        // Load Player Data
        (loadedPlayerData, worked) = LoadPlayerData();

        // If loading worked, use loaded data, otherwise use default data
        if (worked)
        {
            playerData = loadedPlayerData;

            // get all apple count from levels and add them into the global apple count

            int totalApples = 0;
            for (int worldIndex = 0; worldIndex < playerData.worlds.Count; worldIndex++)
            {
                WorldSave world = playerData.worlds[worldIndex];
                for (int missionIndex = 0; missionIndex < world.fruitRecord.Count; missionIndex++)
                {
                    totalApples += world.fruitRecord[missionIndex];
                }
            }
            playerData.appleCount = totalApples;

            SavePlayerData(playerData);
        }
        else
        {
            playerData = defaultData;
            SavePlayerData(playerData);
        }
    }
}
