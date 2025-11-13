using UnityEngine;

public class S_SecretKey : MonoBehaviour
{
    [SerializeField] private AudioClip keyCollected;
    [SerializeField] private GameObject collectVFXPrefab;

    private GameObject collectedVFX;

    private void OnTriggerEnter(Collider other)
    {
        S_LevelManager.instance.AddCollectible("Key", 1);
        AudioManager.instance.PlaySFX(keyCollected);

        collectedVFX = Instantiate(collectVFXPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);

    }
}