using UnityEngine;

public class S_Collectible_Objective : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        S_LevelManager.instance.AddCollectible("Main", 1);
        Destroy(gameObject);
        S_TransitionManager.instance.GoToLevel("HubWorld");
    }
}
