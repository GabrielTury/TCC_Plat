using UnityEngine;

public class S_Mission : MonoBehaviour
{
    [SerializeField]
    private GameObject objective;

    [SerializeField, Tooltip("Game Objects exclusive to this mission")]
    private GameObject[] missionObjects;

    public bool isActive;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isActive == false)
        {
            foreach (GameObject obj in missionObjects)
            {
                //Deactivate all objects on Start
                obj.SetActive(false);
            }
        }
        else
        {
            InitializeMission();
        }
    }
    
    /// <summary>
    /// Initialize all elements of a misison, this must be called AFTER Start logic
    /// </summary>
    public void InitializeMission()
    {
        //Activate this Mission GameObjects
        foreach (GameObject obj in missionObjects)
        {
            obj.SetActive(true);
        }
    }


}
