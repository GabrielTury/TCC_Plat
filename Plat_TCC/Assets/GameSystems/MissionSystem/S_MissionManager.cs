using UnityEngine;
using UnityEngine.SceneManagement;

public class S_MissionManager : MonoBehaviour
{    
    /// <summary>
    /// Array that holds all missions for a certain level, the order in the array is the order of the missions
    /// </summary>
    private S_Mission[] missions;
    
    [SerializeField]
    private Scene level;

    private int missionToActivate;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }


    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectMisison(int selectionIndex)
    {
        missionToActivate = selectionIndex;
    }

    public S_Mission[] GetMissions()
    {
        if (missions == null)
            return null;

        return missions;
    }

    #region LoadingLevel
    private void SceneLoaded(Scene loadedScene, LoadSceneMode loadedSceneMode)
    {
        if(loadedScene == level)
        {
            //missions
        }
    }
    #endregion

}
