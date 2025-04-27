using UnityEngine;

public class S_Collectible : MonoBehaviour
{
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
    }
}
