using System.Collections;
using UnityEngine;

public class S_Collectible : MonoBehaviour
{
    [SerializeField] private AudioClip appleCollected;
    [SerializeField] private GameObject collectVFXPrefab;
    [SerializeField] private int collectibleId = -1;

    private GameObject collectedVFX;

    private MeshRenderer mesh;
    [SerializeField] private Collider collider;
    private void Start()
    {
        mesh = GetComponentInChildren<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        S_LevelManager.instance.AddCollectible("Apple", 1);
        AudioManager.instance.PlaySFX(appleCollected);

        if (collectVFXPrefab != null)
            collectedVFX = Instantiate(collectVFXPrefab, transform.position, Quaternion.identity);

        mesh.enabled = false;
        collider.enabled = false;
        StartCoroutine(DestroyObject());
    }

    private IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(2f);
        Destroy(collectedVFX);
        yield return new WaitForEndOfFrame();
        Destroy(gameObject);
    }
}
