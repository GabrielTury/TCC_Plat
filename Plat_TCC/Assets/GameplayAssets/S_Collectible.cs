using UnityEngine;

public class S_Collectible : MonoBehaviour
{

    //@todo clean the code 
    public GameObject manager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GameObject.Find("Level Manager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        manager.GetComponent<S_LevelManager>().collectibles++;
        Destroy(gameObject);
    }
}
