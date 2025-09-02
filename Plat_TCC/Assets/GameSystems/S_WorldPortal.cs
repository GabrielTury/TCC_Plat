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
    private InputGuideIcons inputGuideIcons;

    [System.Serializable]
    private struct InputGuideIcons
    {
        public Image confirm;
    }

    [SerializeField]
    private UIControllerIcons[] inputIcons; // The icons for each input type

    [SerializeField]
    private SO_MissionUIInfo[] missionInfos;

    [SerializeField]
    private GameObject missionSelectCanvas;

    [SerializeField]
    private SO_WorldInfo worldInfo;

    private void Awake()
    {
        inputs = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        InputSystem.onActionChange += UpdateControllerIcons;
        inputs.Enable();
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= UpdateControllerIcons;
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
            if (inputs.Player.Jump.WasPressedThisFrame() && !hasStartedLoading)
            {
                //loadSize = 1;
                //S_TransitionManager.instance.GoToLevelWithMission(levelName, missionIndex);
                hasStartedLoading = true;

                GameObject.Find("Player").GetComponent<S_PlayerMovement>().PausePlayer();

                GameObject missionCanvas = Instantiate(missionSelectCanvas);
                S_MissionSelectManager missionSelectManager = missionCanvas.GetComponent<S_MissionSelectManager>();
                if (missionSelectManager != null)
                {
                    missionSelectManager.Setup(missionInfos, levelName, worldInfo);
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

    /// <summary>
    /// Sets the controller input guide icons based on the current controller scheme.<br></br>
    /// Is subscribed to and gets input info from the OnUIInputMade event.
    /// </summary>
    private void UpdateControllerIcons(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionPerformed)
        {
            var inputAction = (InputAction)obj;
            var lastControl = inputAction.activeControl;
            var lastDevice = lastControl.device;

            // Or you can check the device type directly
            if (lastDevice is Gamepad)
            {
                // Check for specific gamepad types
                if (lastDevice.description.manufacturer.Contains("Sony"))
                {
                    // PlayStation controller
                    inputGuideIcons.confirm.sprite = inputIcons[2].a;
                }
                else if (lastDevice.description.manufacturer.Contains("Microsoft"))
                {
                    // Xbox controller
                    inputGuideIcons.confirm.sprite = inputIcons[1].a;
                }
                else
                {
                    // Generic gamepad
                    inputGuideIcons.confirm.sprite = inputIcons[1].a;
                }
            }
            else if (lastDevice is Keyboard)
            {
                inputGuideIcons.confirm.sprite = inputIcons[0].a;
            }
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