using UnityEngine;

public class S_Collectible : MonoBehaviour
{
    [SerializeField]
    private int collectibleId = -1;

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

    public void SetId(int id)
    {
        collectibleId = id;
    }

    private void OnTriggerEnter(Collider other)
    {
        manager.GetComponent<S_LevelManager>().collectibles++;
        Destroy(gameObject);
    }
}
