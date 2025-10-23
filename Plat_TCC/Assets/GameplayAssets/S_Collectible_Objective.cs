using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class S_Collectible_Objective : MonoBehaviour
{

    [SerializeField] private AudioClip goalCollected;

    [SerializeField] private GameObject confetti;

    [SerializeField]
    private GameObject handPoint;


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
        other.GetComponentInParent<Animator>().SetTrigger("Victory");


        //S_LevelManager.instance.playerMovement.PausePlayer(true, true);
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(goalCollected);
        }

        Destroy(GetComponent<S_RotateGear>());
        transform.rotation = Quaternion.identity;

        //move gear to player's hand
        transform.SetParent(handPoint.transform);
        transform.localPosition = new Vector3(0,0.005f,0);
        transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);

        StartCoroutine(LevelChangeDelay());

        confetti.SetActive(true);
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
