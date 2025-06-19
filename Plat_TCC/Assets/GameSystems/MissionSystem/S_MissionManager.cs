using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class S_MissionManager : MonoBehaviour
{
    public static S_MissionManager instance;

    [SerializeField]
    /// <summary>
    /// Array that holds all missions for a certain level, the order in the array is the order of the missions
    /// </summary>
    private SO_WorldInfo worldInfo;

    private string[] missionSceneNames;

    private Coroutine Loading;

    private int currentMissionIndex;

    private void Awake()
    {
        instance = this;

        if (worldInfo != null)
            SetWorldInfo(worldInfo);
    }

    public void StartMission(FinishedLoading callback, int missionIndex)
    {
        if(Loading == null)
            Loading = StartCoroutine(MissionLoadTick(callback, missionIndex));
    }

    public delegate void FinishedLoading(bool loadResult);
    /// <summary>
    /// Checks every frame if the frame is loading.
    /// </summary>
    /// <param name="callback">Delegate called when the loading finishes</param>
    /// <param name="missionIndex">Which mission to load based on the index from the world info</param>
    /// <returns></returns>
    private IEnumerator MissionLoadTick(FinishedLoading callback, int missionIndex)
    {
        bool result = false;
        AsyncOperation missionLoad;
        for (int i = 0; i < SceneManager.loadedSceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i); //Check if is not trying to load repetade scene
            if (scene.name == missionSceneNames[missionIndex])
                goto Finish; //Skips Tick Logic and call the callback as false
                
        }

        missionLoad = SceneManager.LoadSceneAsync(missionSceneNames[missionIndex], LoadSceneMode.Additive); //Loads

        while(!missionLoad.isDone)
        {
            //Check if Async Operation is done
            yield return new WaitForEndOfFrame();
        }

        Scene ret = SceneManager.GetSceneByName(missionSceneNames[missionIndex]);//Check if scene was loaded
        if(ret.IsValid())
            result = true;
            
        Finish:
        currentMissionIndex = missionIndex; //Sets the current mission index
        callback?.Invoke(result); //Calls the delegate
    }

    public void SetWorldInfo(SO_WorldInfo input)
    {
        worldInfo = input;
        missionSceneNames = worldInfo.missionSceneNames;
    }

    public void SaveCurrentMissionStatus(bool complete)
    {
        string worldNumber = (worldInfo.worldId + 1).ToString();
        string missionNumber = (currentMissionIndex + 1).ToString();
        
        PlayerPrefs.SetString("Mission" + worldNumber + "-" + missionNumber + "Completed", (complete == true ? "true" : "false"));
        Debug.LogWarning($"Mission {missionNumber} in World {worldNumber} saved as {(complete ? "completed" : "not completed")}. RAW: Mission" + worldNumber + "-" + missionNumber + "Completed");
    }
}
