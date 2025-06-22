using UnityEngine;

public class S_SecretKey : MonoBehaviour
{
    [SerializeField] private AudioClip keyCollected;

    private void OnTriggerEnter(Collider other)
    {
        S_LevelManager.instance.AddCollectible("Key", 1);
        AudioManager.instance.PlaySFX(keyCollected);
        Destroy(gameObject);
    }
}