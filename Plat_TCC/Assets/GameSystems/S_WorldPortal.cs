using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class S_WorldPortal : MonoBehaviour
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

    [SerializeField]
    private InputSystem_Actions inputs;

    private void Awake()
    {
        inputs = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputs.Enable();
    }

    private void OnDisable()
    {
        inputs.Disable();
    }

    void Start()
    {

    }

    void FixedUpdate()
    {
        if (playerIsInRegion)
        {
            //loadSize = Mathf.Lerp(loadSize, 1, 0.02f);
            if (inputs.UI.Submit.WasPressedThisFrame() && !hasStartedLoading)
            {
                //loadSize = 1;
                //S_TransitionManager.instance.GoToLevelWithMission(levelName, missionIndex);
                hasStartedLoading = true;
            }

        }
        else
        {
            //loadSize = Mathf.Lerp(loadSize, 0, 0.02f);
        }

        //loadBar.fillAmount = loadSize;

        //if (loadSize >= 0.99f && hasStartedLoading == false)
        //{
        //    loadSize = 1;
        //    S_TransitionManager.instance.GoToLevelWithMission(levelName, missionIndex);
        //    hasStartedLoading = true;
        //}
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