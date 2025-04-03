using UnityEngine;

public class S_Mission : MonoBehaviour
{
    [SerializeField, Tooltip("Scriptable Object with mission info")]

    private GameObject objective;
    
    private GameObject[] missionCoins;

    private GameObject[] missionObjects;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
            foreach (GameObject obj in missionObjects)
            {
                //Deactivate all objects on Start
                obj.SetActive(false);
            }
    }
    
    /// <summary>
    /// Initialize all elements of a misison, this must be called AFTER Start logic
    /// </summary>
    public void InitializeMission(SO_MissionInfo missionInfo)
    {
        objective = missionInfo.objective;
        missionCoins = missionInfo.missionCoins;
        missionObjects = missionInfo.missionObjects;
        //Activate this Mission GameObjects
        foreach (GameObject obj in missionObjects)
        {
            obj.SetActive(true);
        }

        foreach (GameObject obj in missionCoins)
        {
            obj.SetActive(true);
        }
        
    }


}
