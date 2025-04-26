using UnityEngine;

public class S_Respawn_Fall : MonoBehaviour
{
    public GameObject transitionManager;

    void Start()
    {
        transitionManager = GameObject.Find("TransitionCanvas");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death Zone"))
        {
            transitionManager.GetComponent<S_TransitionManager>().RestartLevel();
        }
    }
}
