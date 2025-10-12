using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using HF = S_HelperFunctions;

public class S_PauseManager : MonoBehaviour, IMenuCaller
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private RectTransform mainPanel;

    [SerializeField]
    private RawImage shadowScroller;

    [SerializeField]
    private Image gameLogo;

    [SerializeField]
    private Button[] buttons = new Button[5];

    //[SerializeField]
    private Image[] buttonsImage = new Image[5];

    [SerializeField]
    private int selectionIndex = 0;

    //[SerializeField]
    private Coroutine[] buttonAnimationCoroutine = new Coroutine[7];

    [SerializeField]
    private int lastButtonHighlighted = 0;

    [SerializeField]
    private RectTransform collectibleBG;

    [SerializeField]
    private GameObject collectibleShowcasePrefab;

    [SerializeField]
    private List<GameObject> collectibleShowcases = new List<GameObject>();

    [SerializeField]
    private InputSystem_Actions inputs;

    [SerializeField]
    private RectTransform mainHolder;

    private bool isThisMenuActive = true;
    private bool isInPause = false;

    //[SerializeField]
    //private float offset = 800f;

    private Coroutine mainMenuAnimationCoroutine;
    private Coroutine mainHolderAnimationCoroutine;
    private Coroutine collectibleBGAnimationCoroutine;

    [SerializeField]
    private InputGuideIcons inputGuideIcons;

    [System.Serializable]
    private struct InputGuideIcons
    {
        public Image navigateUpDown;
        public Image confirm;
        public Image cancel;
    }

    [SerializeField]
    private UIControllerIcons[] inputIcons; // The icons for each input type

    private void Awake()
    {
        inputs = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        //GameEvents.UIInputMade += UpdateControllerIcons;
        InputSystem.onActionChange += UpdateControllerIcons;
        inputs.Enable();
    }

    private void OnDisable()
    {
        //GameEvents.UIInputMade -= UpdateControllerIcons;
        InputSystem.onActionChange -= UpdateControllerIcons;
        inputs.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeContent();

        for (int i = 0; i < buttons.Length; i++)
        {
            HighlightButton(i);
        }
        HighlightButton(selectionIndex);

        S_LevelManager.instance.OnKeyCollected += UpdateCollectibleTrackingKey;
        S_LevelManager.instance.OnGearCollected += UpdateCollectibleTrackingGear;
        S_LevelManager.instance.OnAppleCollected += UpdateCollectibleTrackingApple;

        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    void Update()
    {
        CheckPlayerInput();
        //ManageOffset();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ScrollBackgroundShadow(0.001f);
    }

    /*
    private void ManageOffset()
    {
        if (isInPause)
        {
            offset = Mathf.Lerp(offset, 0, Time.deltaTime * 2f);
        }
        else
        {
            offset = Mathf.Lerp(offset, -800, Time.deltaTime * 2f);
        }
    }
    */

    private void OnSceneChanged(Scene current, Scene next)
    {
        // reset collectible tracking

        foreach (var item in collectibleShowcases)
        {
            Destroy(item);
        }
        collectibleShowcases.Clear();
    }

    private void InitializeContent()
    {
        mainHolder.anchoredPosition = new Vector2(-1900, 0);
        //mainPanel.anchoredPosition = new Vector2(-800, 0);
        //shadowScroller.rectTransform.anchoredPosition = new Vector2(-1615, 0);

        //gameLogo.rectTransform.anchoredPosition = new Vector2(-570, 750);
        //gameLogo.rectTransform.localScale = new Vector2(0, 0);

        //int distance = 140;
        for (int i = 0; i < buttons.Length; i++)
        {
            buttonsImage[i] = buttons[i].GetComponent<Image>();
        }
        //for (int i = 0; i < buttons.Length; i++)
        //{
        //    buttonsImage[i].rectTransform.anchoredPosition = new Vector2(-570, -800 - ((i + 1) * distance));
        //}
        
    }
    
    private void BeginSceneAnimation()
    {
        StartCoroutine(HF.SmoothRawMove(shadowScroller, new Vector2(-550, 0), 1.5f));

        StartCoroutine(HF.SmoothMove(gameLogo, new Vector2(-570, 315), 1.5f));
        StartCoroutine(HF.SmoothScale(gameLogo, new Vector2(1f, 1f), 1.5f));

        // Move the buttons to the left
        int distance = 140;
        float[] timeDiff = new float[4] { 1.5f, 1.8f, 2.1f, 2.4f };

        for (int i = 0; i < buttons.Length; i++)
        {
            StartCoroutine(HF.SmoothMove(buttonsImage[i], new Vector2(-570, 0 - (i * distance)), timeDiff[i]));
        }
    }

    private void ScrollBackgroundShadow(float speed = 1)
    {
        shadowScroller.uvRect = new Rect(0, shadowScroller.uvRect.y - speed, 1, 1);
    }

    private void CheckPlayerInput()
    {
        if (inputs.UI.Pause.WasPressedThisFrame())
        {
            if (isInPause)
            {
                if (isThisMenuActive)
                {
                    if (mainPanel.anchoredPosition.x > -200)
                    {
                        ResumeGame();
                    }
                }
                else
                {
                    //if (mainPanel.anchoredPosition.x > -200)
                    //{
                    //    ResumeOperation();
                    //}
                }
            }
            else
            {
                ShowMenu();
            }
        }

        if (inputs.UI.Navigate.WasPressedThisFrame() && isInPause && isThisMenuActive)
        {
            if (inputs.UI.Navigate.ReadValue<Vector2>().y > 0)
            {
                selectionIndex--;
                if (selectionIndex < 0)
                {
                    selectionIndex = buttons.Length - 1;
                }
                HighlightButton(selectionIndex);
            }
            else if (inputs.UI.Navigate.ReadValue<Vector2>().y < 0)
            {
                selectionIndex++;
                if (selectionIndex >= buttons.Length)
                {
                    selectionIndex = 0;
                }
                HighlightButton(selectionIndex);
            }
        }
        
        if (inputs.UI.Submit.WasPressedThisFrame() && isInPause && isThisMenuActive && mainPanel.anchoredPosition.x > -200)
        {
            buttons[selectionIndex].onClick.Invoke();
        }
    }


    /// <summary>
    /// For item types:<br></br>
    /// 1 = Apple;
    /// 2 = Key;
    /// 3 = Gear<br></br>
    /// If maxAmount is defined, it will show the required amount in the UI rather than just the current amount collected.
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="maxAmount"></param>
    public void AddCollectibleTracking(int itemType, int maxAmount = -1)
    {
        GameObject newCollectible = Instantiate(collectibleShowcasePrefab, collectibleBG.transform);
        newCollectible.transform.GetChild(0).GetComponent<Image>().sprite = itemType == 1 ? S_CollectibleExhibitor.instance.appleSprite : itemType == 2 ? S_CollectibleExhibitor.instance.keySprite : itemType == 3 ? S_CollectibleExhibitor.instance.gearSprite : null;
        if (maxAmount > 0)
        {
            newCollectible.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = itemType == 1 ? "Apples: 0/" + maxAmount : itemType == 2 ? "Keys: 0/" + maxAmount : itemType == 3 ? "Gears: 0/" + maxAmount : "Unknown";
        }
        else
        {
            newCollectible.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = itemType == 1 ? "Apples: 0x" : itemType == 2 ? "Keys: 0x" : itemType == 3 ? "Gears: 0x" : "Unknown";
        }
        collectibleShowcases.Add(newCollectible);
    }

    private void UpdateCollectibleTrackingApple(int count) { UpdateCollectibleTracking(1, count); }
    private void UpdateCollectibleTrackingKey(int count) { UpdateCollectibleTracking(2, count); }
    private void UpdateCollectibleTrackingGear(int count) { UpdateCollectibleTracking(3, count); }

    public void UpdateCollectibleTracking(int itemType, int count)
    {
        // Map itemType to name and format
        string itemName = itemType == 1 ? "Apples" : itemType == 2 ? "Keys" : itemType == 3 ? "Gears" : null;
        if (itemName == null) return;

        for (int i = 0; i < collectibleShowcases.Count; i++)
        {
            var textComp = collectibleShowcases[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            if (textComp.text.Contains(itemName))
            {
                if (textComp.text.Contains("/"))
                {
                    int slashIdx = textComp.text.IndexOf('/');
                    string maxAmount = textComp.text.Substring(slashIdx + 1);
                    textComp.text = $"{itemName}: {count}/{maxAmount}";
                }
                else
                {
                    textComp.text = $"{itemName}: {count}x";
                }
                return;
            }
        }
    }

    public void HighlightButton(int buttonIndex)
    {
        if (buttonAnimationCoroutine[lastButtonHighlighted] != null)
        {
            StopCoroutine(buttonAnimationCoroutine[lastButtonHighlighted]);
            buttonAnimationCoroutine[lastButtonHighlighted] = StartCoroutine(UnHighlightAnimation(lastButtonHighlighted));
        }
        if (buttonAnimationCoroutine[buttonIndex] != null)
        {
            StopCoroutine(buttonAnimationCoroutine[buttonIndex]);
        }
        buttonAnimationCoroutine[buttonIndex] = StartCoroutine(HighlightAnimation(buttonIndex));
        lastButtonHighlighted = buttonIndex;
        selectionIndex = buttonIndex;
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
                    inputGuideIcons.navigateUpDown.sprite = inputIcons[2].updown;
                    inputGuideIcons.confirm.sprite = inputIcons[2].a;
                    inputGuideIcons.cancel.sprite = inputIcons[2].b;
                }
                else if (lastDevice.description.manufacturer.Contains("Microsoft"))
                {
                    // Xbox controller
                    inputGuideIcons.navigateUpDown.sprite = inputIcons[1].updown;
                    inputGuideIcons.confirm.sprite = inputIcons[1].a;
                    inputGuideIcons.cancel.sprite = inputIcons[1].b;
                }
                else
                {
                    // Generic gamepad
                    inputGuideIcons.navigateUpDown.sprite = inputIcons[1].updown;
                    inputGuideIcons.confirm.sprite = inputIcons[1].a;
                    inputGuideIcons.cancel.sprite = inputIcons[1].b;
                }
            }
            else if (lastDevice is Keyboard)
            {
                inputGuideIcons.navigateUpDown.sprite = inputIcons[0].updown;
                inputGuideIcons.confirm.sprite = inputIcons[0].a;
                inputGuideIcons.cancel.sprite = inputIcons[0].b;
            }
        }
    }

    private IEnumerator HighlightAnimation(int buttonIndex)
    {
        Image objImage = buttonsImage[buttonIndex];
        Color32 startColor = objImage.color;
        Vector2 startScale = objImage.rectTransform.localScale;
        float lerp = 0;
        float smoothLerp = 0;
        float duration = 0.2f;
        Color32 targetColor = new Color32(255, 255, 255, 255);
        Vector2 targetScale = new Vector2(1f, 1f);

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.unscaledDeltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            objImage.color = Color32.Lerp(startColor, targetColor, smoothLerp);
            objImage.rectTransform.localScale = Vector2.Lerp(startScale, targetScale, smoothLerp);
            yield return null;
        }

        objImage.color = targetColor;
        objImage.rectTransform.localScale = targetScale;
    }

    private IEnumerator UnHighlightAnimation(int buttonIndex)
    {
        Image objImage = buttonsImage[buttonIndex];
        Color32 startColor = objImage.color;
        Vector2 startScale = objImage.rectTransform.localScale;
        float lerp = 0;
        float smoothLerp = 0;
        float duration = 0.2f;
        Color32 targetColor = new Color32(255, 255, 255, 50);
        Vector2 targetScale = new Vector2(0.85f, 0.85f);

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.unscaledDeltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            objImage.color = Color32.Lerp(startColor, targetColor, smoothLerp);
            objImage.rectTransform.localScale = Vector2.Lerp(startScale, targetScale, smoothLerp);
            yield return null;
        }

        objImage.color = targetColor;
        objImage.rectTransform.localScale = targetScale;
    }

    #region Buttons Behavior

    public void RestartAtLastCheckpoint()
    {
        if (SceneManager.GetActiveScene().name == "HubWorld")
        {
            Debug.Log("You are in the Hub World, cannot restart level.");
            return;
        }
        //S_LevelManager.instance.ResetLevel();
        S_TransitionManager.instance.RestartLevel();
        ResumeGame();
    }

    public void ReturnToWorldHub()
    {
        if (SceneManager.GetActiveScene().name == "HubWorld")
        {
            Debug.Log("You are in the Hub World, cannot restart level.");
            return;
        }
        S_TransitionManager.instance.GoToLevel("HubWorld");
        ResumeGame();
    }
    public void ResumeOperation()
    {
        isThisMenuActive = true;
        if (mainMenuAnimationCoroutine != null)
        {
            StopCoroutine(mainMenuAnimationCoroutine);
        }
        mainMenuAnimationCoroutine = StartCoroutine(HF.SmoothRectMove(mainPanel, new Vector2(0, 0), 0.3f));
    }

    public void ShowMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        isInPause = true;
        Time.timeScale = 0f;
        if (mainHolderAnimationCoroutine != null)
        {
            StopCoroutine(mainHolderAnimationCoroutine);
        }
        if (collectibleBGAnimationCoroutine != null)
        {
            StopCoroutine(collectibleBGAnimationCoroutine);
        }
        mainHolderAnimationCoroutine = StartCoroutine(HF.SmoothRectMove(mainHolder, new Vector2(0, 0), 0.3f));
        collectibleBGAnimationCoroutine = StartCoroutine(HF.SmoothRectMove(collectibleBG, new Vector2(720, -375), 0.3f));
    }

    public void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        isInPause = false;
        Time.timeScale = 1f;
        if (mainHolderAnimationCoroutine != null)
        {
            StopCoroutine(mainHolderAnimationCoroutine);
        }
        if (collectibleBGAnimationCoroutine != null)
        {
            StopCoroutine(collectibleBGAnimationCoroutine);
        }
        mainHolderAnimationCoroutine = StartCoroutine(HF.SmoothRectMove(mainHolder, new Vector2(-1900, 0), 0.3f));
        collectibleBGAnimationCoroutine = StartCoroutine(HF.SmoothRectMove(collectibleBG, new Vector2(720, -800), 0.3f));
    }

    public void OpenSettingsMenu()
    {
        if (mainMenuAnimationCoroutine != null)
        {
            StopCoroutine(mainMenuAnimationCoroutine);
        }
        S_SettingsManager.instance.OpenSettings(this);
        S_SettingsManager.instance.isOnSettingsMenu = true;
        mainMenuAnimationCoroutine = StartCoroutine(HF.SmoothRectMove(mainPanel, new Vector2(-800, 0), 0.3f));
        isThisMenuActive = false;
    }

    public void QuitToTitle()
    {
        isInPause = false;
        Time.timeScale = 1f;
        if (mainHolderAnimationCoroutine != null)
        {
            StopCoroutine(mainHolderAnimationCoroutine);
        }
        mainHolderAnimationCoroutine = StartCoroutine(HF.SmoothRectMove(mainHolder, new Vector2(-1900, 0), 0.3f));
        S_TransitionManager.instance.GoToLevel("MainMenu");
        Debug.Log("Quitting to Title Screen...");
    }
    #endregion
}
