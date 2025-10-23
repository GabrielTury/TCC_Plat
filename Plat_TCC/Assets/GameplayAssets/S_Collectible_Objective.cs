using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class S_Collectible_Objective : MonoBehaviour
{

    [SerializeField] private AudioClip goalCollected;

    private void OnTriggerEnter(Collider other)
    {
        S_LevelManager.instance.AddCollectible("Main", 1);
        if (S_SaveManager.instance != null)
        {
            S_MissionManager.instance.SaveCurrentMissionStatus(true);
        }
        else
        {
            Debug.LogWarning("Mission not saved");
        }
        
        other.GetComponentInParent<S_PlayerMovement>().PausePlayer();
        other.GetComponentInParent<Rigidbody>().linearVelocity = Vector3.zero;//@todo colocar um metodo de gente
        //S_LevelManager.instance.playerMovement.PausePlayer(true, true);
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(goalCollected);
        }

        other.GetComponentInParent<Animator>().SetTrigger("Victory");

        StartCoroutine(LevelChangeDelay());

        GetComponent<MeshRenderer>().enabled = false;
    }

    private IEnumerator LevelChangeDelay()
    {
        yield return new WaitForSeconds(3);
        if (S_TransitionManager.instance != null)
        {
            S_TransitionManager.instance.GoToLevel("HubWorld");
        }
        else
        {
            SceneManager.LoadScene("HubWorld");
        }
        
    }
}
