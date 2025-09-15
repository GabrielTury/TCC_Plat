using UnityEngine;

public class S_FinalCage : MonoBehaviour
{

    [SerializeField, Range(1, 5)]
    private int keyAmount = 1;

    bool isCageReady = false; //Is the cage ready to be opened?


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        S_LevelManager.instance.OnKeyCollected += KeyCollected;
    }

    private void KeyCollected()
    {
        if (S_LevelManager.instance.keyCollectibles >= keyAmount)
        {
            S_LevelManager.instance.OnKeyCollected -= KeyCollected;
            isCageReady = true;
        }
    }

    private void OnDisable()
    {
        S_LevelManager.instance.OnKeyCollected -= KeyCollected;
    }

    private void OnDestroy()
    {
        S_LevelManager.instance.OnKeyCollected -= KeyCollected;
    }

    //checks if ready to be opened and then opens it
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (isCageReady)
            {
                Destroy(gameObject);
            }
        }
    }
}
