using UnityEngine;

public class S_FinalCage : MonoBehaviour
{

    [SerializeField, Range(1, 5)]
    private int keyAmount = 1;

    bool isGateReady = false;
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
            isGateReady = true;
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (isGateReady)
            {
                Destroy(gameObject);
            }
        }
    }
}
