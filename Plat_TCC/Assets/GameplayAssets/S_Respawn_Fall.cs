using UnityEngine;

public class S_Respawn_Fall : MonoBehaviour
{
    [SerializeField] private GameObject transitionManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death Zone"))
        {
            transitionManager.GetComponent<S_TransitionManager>().RestartLevel();
        }
    }
}
