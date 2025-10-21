using System;
using System.Collections;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
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

    public enum SkyTime
    {
        Morning = 0,
        Evening = 1,
        Night = 2
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this);

        if (worldInfo != null)
            SetWorldInfo(worldInfo);
    }

    public void StartMission(FinishedLoading callback, int missionIndex, SkyTime time = SkyTime.Morning)
    {
        if(Loading == null)
            Loading = StartCoroutine(MissionLoadTick(callback, missionIndex, time));
    }

    public delegate void FinishedLoading(bool loadResult);
    /// <summary>
    /// Checks every frame if the frame is loading.
    /// </summary>
    /// <param name="callback">Delegate called when the loading finishes</param>
    /// <param name="missionIndex">Which mission to load based on the index from the world info</param>
    /// <returns></returns>
    private IEnumerator MissionLoadTick(FinishedLoading callback, int missionIndex, SkyTime time = SkyTime.Morning)
    {
        if (SceneManager.GetActiveScene().name == "HubWorld" || SceneManager.GetActiveScene().name == "MainMenu") { yield break; }
        bool result = false;
        AsyncOperation missionLoad;
        for (int i = 0; i < SceneManager.loadedSceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i); //Check if is not trying to load repetade scene
            if (scene.name == missionSceneNames[missionIndex])
                goto Finish; //Skips Tick Logic and call the callback as false

        }        
        SetSky(worldInfo.levelLightInfos[(int)time]);


        missionLoad = SceneManager.LoadSceneAsync(missionSceneNames[missionIndex], LoadSceneMode.Additive); //Loads

        while(!missionLoad.isDone)
        {
            //Check if Async Operation is done
            yield return new WaitForEndOfFrame();
        }
        
        Scene ret = SceneManager.GetSceneByName(missionSceneNames[missionIndex]);//Check if scene was loaded
        if(ret.IsValid())
        {
            result = true;            
        }

        Finish:
        currentMissionIndex = missionIndex; //Sets the current mission index
        callback?.Invoke(result); //Calls the delegate
        Loading = null;
    }

    public void SetWorldInfo(SO_WorldInfo input)
    {
        worldInfo = input;
        missionSceneNames = worldInfo.missionSceneNames;
    }

    public void SaveCurrentMissionStatus(bool complete)
    {
        int worldNumber = (worldInfo.worldId + 1);
        int missionNumber = (currentMissionIndex + 1);

        S_SaveManager.instance.SetMissionStatus(worldNumber, missionNumber, complete);
        //PlayerPrefs.SetString("Mission" + worldNumber + "-" + missionNumber + "Completed", (complete == true ? "true" : "false"));
        Debug.LogWarning($"Mission {missionNumber} in World {worldNumber} saved as {(complete ? "completed" : "not completed")}. RAW: Mission" + worldNumber + "-" + missionNumber + "Completed");
    }

    public void SetSky(LevelLightInfo lightInfo)
    {
        Light mainLight = FindFirstObjectByType<Light>();
        mainLight.color = lightInfo.lightColor;
        mainLight.intensity = lightInfo.intensity;
        mainLight.transform.rotation = lightInfo.directionalLightRotation;

        Material skyboxMaterial = lightInfo.skybox;

        if (skyboxMaterial != null)
        {
            UnityEngine.RenderSettings.skybox = skyboxMaterial;
            DynamicGI.UpdateEnvironment();
        }
        else
        {
            Debug.LogWarning("Skybox material is null. Cannot set skybox.");
        }
    }
}
