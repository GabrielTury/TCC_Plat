using UnityEngine;

public class S_Collectible_Objective : MonoBehaviour
{

    public GameObject manager;
    public GameObject transitionManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GameObject.Find("Level Manager");
        transitionManager = GameObject.Find("TransitionCanvas");
    }

    private void OnTriggerEnter(Collider other)
    {
        manager.GetComponent<S_LevelManager>().mainCollectibles++;
        Destroy(gameObject);
        transitionManager.GetComponent<S_TransitionManager>().GoToLevel("HubWorld");
    }
}
