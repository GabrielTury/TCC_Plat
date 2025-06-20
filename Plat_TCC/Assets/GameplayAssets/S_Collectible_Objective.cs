using UnityEngine;

public class S_Collectible_Objective : MonoBehaviour
{

    [SerializeField] private AudioClip goalCollected;

    private void OnTriggerEnter(Collider other)
    {
        S_LevelManager.instance.AddCollectible("Main", 1);
        S_MissionManager.instance.SaveCurrentMissionStatus(true);
        AudioManager.instance.PlaySFX(goalCollected);
        Destroy(gameObject);
        S_TransitionManager.instance.GoToLevel("HubWorld");
    }
}
