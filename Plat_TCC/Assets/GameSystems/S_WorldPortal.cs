using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class S_WorldPortal : MonoBehaviour
{
    [SerializeField]
    private string levelName;

    //[SerializeField]
    //private int missionIndex;

    private bool playerIsInRegion = false;

    //private float loadSize = 0;

    private bool hasStartedLoading = false;

    //[SerializeField]
    //private Image loadBar;

    [SerializeField]
    private InputSystem_Actions inputs;

    [SerializeField]
    private SO_MissionUIInfo[] missionInfos;

    [SerializeField]
    private GameObject missionSelectCanvas;

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

    void Update()
    {
        if (playerIsInRegion)
        {
            //loadSize = Mathf.Lerp(loadSize, 1, 0.02f);
            if (inputs.Player.Interact.WasPressedThisFrame() && !hasStartedLoading)
            {
                //loadSize = 1;
                //S_TransitionManager.instance.GoToLevelWithMission(levelName, missionIndex);
                hasStartedLoading = true;
                GameObject missionCanvas = Instantiate(missionSelectCanvas);
                S_MissionSelectManager missionSelectManager = missionCanvas.GetComponent<S_MissionSelectManager>();
                if (missionSelectManager != null)
                {
                    missionSelectManager.Setup(missionInfos, levelName);
                }
                else
                {
                    Debug.LogError("S_MissionSelectManager component not found on the instantiated canvas.");
                }
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