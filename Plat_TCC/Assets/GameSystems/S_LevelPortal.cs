using UnityEngine;
using UnityEngine.UI;

public class S_LevelPortal : MonoBehaviour
{
    [SerializeField]
    private string levelName;

    [SerializeField]
    private int missionIndex;

    private bool playerIsInRegion = false;

    private float loadSize = 0;

    private bool hasStartedLoading = false;

    [SerializeField]
    private Image loadBar;

    void Start()
    {

    }

    void FixedUpdate()
    {
        if (playerIsInRegion)
        {
            loadSize = Mathf.Lerp(loadSize, 1, 0.02f);
        }
        else
        {
            loadSize = Mathf.Lerp(loadSize, 0, 0.02f);
        }

        loadBar.fillAmount = loadSize;

        if (loadSize >= 0.99f && hasStartedLoading == false)
        {
            loadSize = 1;
            //S_TransitionManager.instance.GoToLevelWithMission(levelName, missionIndex, worldInfo);
            hasStartedLoading = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.CompareTag("Player"))
        {
            playerIsInRegion = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.CompareTag("Player"))
        {
            playerIsInRegion = false;
        }
    }
}