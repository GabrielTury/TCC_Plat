using UnityEngine;
using UnityEngine.Audio;

public class S_Collectible : MonoBehaviour
{

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip AppleCollected;
    [SerializeField]
    private int collectibleId = -1;
   
    public void SetId(int id)
    {
        collectibleId = id;
    }

    private void OnTriggerEnter(Collider other)
    {
        S_LevelManager.instance.AddCollectible("Apple", 1);
        Destroy(gameObject);
        audioSource.PlayOneShot(AppleCollected);

    }
}
