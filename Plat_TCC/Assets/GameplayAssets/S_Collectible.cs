using UnityEngine;

public class S_Collectible : MonoBehaviour
{
    [SerializeField] private AudioClip appleCollected;
    [SerializeField] private GameObject collectVFXPrefab;
    [SerializeField] private int collectibleId = -1;

    public void SetId(int id) => collectibleId = id;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        S_LevelManager.instance.AddCollectible("Apple", 1);
        AudioManager.instance.PlaySFX(appleCollected);

        if (collectVFXPrefab != null)
            Instantiate(collectVFXPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
