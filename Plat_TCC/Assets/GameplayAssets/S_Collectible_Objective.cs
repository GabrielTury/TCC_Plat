using System.Collections;
using UnityEngine;

public class S_Collectible_Objective : MonoBehaviour
{

    [SerializeField] private AudioClip goalCollected;

    private void OnTriggerEnter(Collider other)
    {
        S_LevelManager.instance.AddCollectible("Main", 1);
        S_MissionManager.instance.SaveCurrentMissionStatus(true);
        other.GetComponentInParent<S_PlayerMovement>().PausePlayer();
        other.GetComponentInParent<Rigidbody>().linearVelocity = Vector3.zero;//@todo colocar um metodo de gente
        //S_LevelManager.instance.playerMovement.PausePlayer(true, true);
        AudioManager.instance.PlaySFX(goalCollected);
        StartCoroutine(LevelChangeDelay());

        GetComponent<MeshRenderer>().enabled = false;
    }

    private IEnumerator LevelChangeDelay()
    {
        yield return new WaitForSeconds(3);
        S_TransitionManager.instance.GoToLevel("HubWorld");
    }
}
