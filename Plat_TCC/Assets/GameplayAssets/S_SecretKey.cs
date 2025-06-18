using UnityEngine;

public class S_SecretKey : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        S_LevelManager.instance.AddCollectible("Key", 1);
        Destroy(gameObject);
    }
}