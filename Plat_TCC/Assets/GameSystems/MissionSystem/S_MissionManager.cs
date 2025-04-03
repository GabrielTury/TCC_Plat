using UnityEngine;
using UnityEngine.SceneManagement;

public class S_MissionManager : MonoBehaviour
{
    /// <summary>
    /// Array that holds all missions for a certain level, the order in the array is the order of the missions
    /// </summary>

    private SO_MissionInfo[] missions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
