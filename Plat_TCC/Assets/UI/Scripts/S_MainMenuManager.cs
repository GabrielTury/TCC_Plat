using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using HF = S_HelperFunctions;

public class S_MainMenuManager : MonoBehaviour, IMenuCaller
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
    private List<Button> buttons = new List<Button>();

    [SerializeField]
    private List<Image> buttonsImage = new List<Image>();  

    [SerializeField]
    private Button buttonLanguage;

    [SerializeField]
    private int selectionIndex = 0;

    private string currentLanguage = "en";

    [SerializeField]
    private Sprite[] flagImages;

    [SerializeField]
    private Image flagIcon;

    //[SerializeField]
    private Coroutine[] buttonAnimationCoroutine = new Coroutine[5];

    [SerializeField]
    private int lastButtonHighlighted = 0;

    [SerializeField]
    private InputSystem_Actions inputs;

    private bool isThisMenuActive = true;

    private Coroutine mainMenuAnimationCoroutine;

    [SerializeField]
    private InputGuideIcons inputGuideIcons;

    [SerializeField]
    private TextMeshProUGUI[] inputGuideText;

    [System.Serializable]
    private struct InputGuideIcons
    {
        public Image navigateUpDown;
        public Image confirm;
        public Image cancel;
    }

    [SerializeField]
    private UIControllerIcons[] inputIcons; // The icons for each input type

    [SerializeField]
    private TextMeshProUGUI creditsText;

    [SerializeField, TextArea]
    private string creditsContentEn = "Game developed by:\n\nJohn Doe\nJane Smith\n\nSpecial Thanks to:\nCommunity Contributors\nOpen Source Libraries";

    [SerializeField, TextArea]
    private string creditsContentBr = "Jogo desenvolvido por:\n\nJohn Doe\nJane Smith\n\nAgradecimentos especiais a:\nColaboradores da comunidade\nBibliotecas de código aberto";

    private bool isInCredits = false;

    [SerializeField]
    private GameObject creditsPanel;

    private void Awake()
    {
        inputs = new InputSystem_Actions();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f; // Ensure the time scale is set to normal
        InitializeSceneObjects();
        BeginSceneAnimation();
        for (int i = 0; i < buttons.Count; i++)
        {
            HighlightButton(i);
        }
        HighlightButton(selectionIndex);

        Debug.LogWarning(S_SaveManager.instance);

        S_SaveManager.SettingsData settings = S_SaveManager.instance.GetSettingsData();

        currentLanguage = settings.language;

        Debug.LogWarning("CURRENT LANGUAGE: " + currentLanguage);
        RefreshLanguage();

        if (buttons[0] == null)
        {
            HighlightButton(1);
        }
        else
        {
            HighlightButton(0);
        }
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

    void Update()
    {
        CheckPlayerInput();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ScrollBackgroundShadow(0.001f);
    }

    void DetectMouseMovement()
    {
        if (Mouse.current.delta.ReadValue().magnitude > 0.1f)
        {
            //Debug.Log("Mouse moved");

            inputGuideIcons.navigateUpDown.sprite = inputIcons[0].updown;
            inputGuideIcons.confirm.sprite = inputIcons[0].a;
            inputGuideIcons.cancel.sprite = inputIcons[0].b;
        }
    }

    private void InitializeSceneObjects()
    {
        mainCamera.transform.position = new Vector3(0, 6, 35);
        mainCamera.transform.eulerAngles = new Vector3(-70, 0, 0);

        shadowScroller.rectTransform.anchoredPosition = new Vector2(-1615, 0);

        gameLogo.rectTransform.anchoredPosition = new Vector2(-570, 750);
        gameLogo.rectTransform.localScale = new Vector2(0, 0);

        creditsPanel.SetActive(false);

        int distance = 140;

        Debug.LogWarning("buttons count " + buttons.Count);

        for (int i = 0; i < buttons.Count; i++)
        {
            if (i < buttonsImage.Count && buttonsImage[i] != null)
            {
                buttonsImage[i] = buttons[i].GetComponent<Image>();
            }
        }
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i < buttonsImage.Count && buttonsImage[i] != null)
            {
                buttonsImage[i].rectTransform.anchoredPosition = new Vector2(-570, -800 - ((i + 1) * distance));
            }
        }
        if (ArePlayerDataEqual(S_SaveManager.instance.GetPlayerData(), S_SaveManager.instance.defaultData))
        {
            if (buttons.Count > 0)
            {
                Destroy(buttons[0].gameObject);
                buttons[0] = null; // Disable Continue Game button
                if (buttonsImage.Count > 0)
                {
                    buttonsImage[0] = null;
                }
            }
            Debug.Log("Player data matches the default data.");
        }
    }
    
    private void BeginSceneAnimation()
    {
        StartCoroutine(HF.SmoothMoveCamera(mainCamera, new Vector3(-1, 1, 45), 5));
        StartCoroutine(HF.SmoothRotateCamera(mainCamera, new Vector3(30+360, 50, 0), 5));

        // Move the shadow scroller to the right
        StartCoroutine(HF.SmoothRawMove(shadowScroller, new Vector2(-550, 0), 1.5f));

        StartCoroutine(HF.SmoothMove(gameLogo, new Vector2(-570, 315), 1.5f));
        StartCoroutine(HF.SmoothScale(gameLogo, new Vector2(1f, 1f), 1.5f));

        // Move the buttons to the left
        int distance = 100;
        float[] timeDiff = new float[5] { 1.5f, 1.8f, 2.1f, 2.4f, 2.6f };

        for (int i = 0; i < buttons.Count; i++)
        {
            if (i < buttonsImage.Count && buttonsImage[i] != null)
            {
                StartCoroutine(HF.SmoothMove(buttonsImage[i], new Vector2(-570, 0 - (i * distance)), timeDiff[i]));
            }
        }
    }

    private void ScrollBackgroundShadow(float speed = 1)
    {
        shadowScroller.uvRect = new Rect(0, shadowScroller.uvRect.y - speed, 1, 1);
    }

    private void CheckPlayerInput()
    {
        DetectMouseMovement();
        if (Time.timeSinceLevelLoad < 1.5f || isThisMenuActive == false || mainPanel.anchoredPosition.x < -100)
        {
            return;
        }
        if (inputs.Player.Crouch.WasPressedThisFrame())
        {
            ChangeLanguage();
        }
        if (inputs.UI.Navigate.WasPressedThisFrame() & !isInCredits)
        {
            if (inputs.UI.Navigate.ReadValue<Vector2>().y > 0)
            {
                selectionIndex--;
                if (selectionIndex < 0)
                {
                    selectionIndex = buttons.Count - 1;
                }
                if (buttons[selectionIndex] == null)
                {
                    selectionIndex--;
                }
                if (selectionIndex < 0)
                {
                    selectionIndex = buttons.Count - 1;
                }
                HighlightButton(selectionIndex);
            }
            else if (inputs.UI.Navigate.ReadValue<Vector2>().y < 0)
            {
                selectionIndex++;
                if (selectionIndex >= buttons.Count)
                {
                    selectionIndex = 0;
                }
                if (buttons[selectionIndex] == null)
                {
                    selectionIndex++;
                }
                if (selectionIndex >= buttons.Count)
                {
                    if (buttons[0] == null)
                    {
                        selectionIndex = 1;
                    }
                    else
                    {
                        selectionIndex = 0;
                    }
                }
                HighlightButton(selectionIndex);
            }
        }

        if (inputs.UI.Submit.WasPressedThisFrame() && selectionIndex >= 0 && selectionIndex < buttons.Count & !isInCredits)
        {
            if (buttons[selectionIndex] != null)
            {
                buttons[selectionIndex].onClick.Invoke();
            }
        }

        if (isInCredits && inputs.UI.Cancel.WasPressedThisFrame())
        {
            isInCredits = false;
            creditsPanel.SetActive(false);
        }
    }

    private void HighlightButton(int buttonIndex)
    {
        if (isInCredits)
        {
            return;
        }
        if (buttonIndex < 0 || buttonIndex >= buttons.Count || buttons[buttonIndex] == null)
        {
            Debug.LogWarning($"Invalid button index: {buttonIndex}. Skipping highlight.");
            return;
        }

        // Defensive: Check lastButtonHighlighted is valid before using it
        if (lastButtonHighlighted >= 0 && lastButtonHighlighted < buttonAnimationCoroutine.Length && buttonAnimationCoroutine[lastButtonHighlighted] != null)
        {
            StopCoroutine(buttonAnimationCoroutine[lastButtonHighlighted]);
            buttonAnimationCoroutine[lastButtonHighlighted] = StartCoroutine(UnHighlightAnimation(lastButtonHighlighted));
        }

        if (buttonIndex >= 0 && buttonIndex < buttonAnimationCoroutine.Length && buttonAnimationCoroutine[buttonIndex] != null)
        {
            StopCoroutine(buttonAnimationCoroutine[buttonIndex]);
        }

        // Defensive: Check buttonsImage size and null before accessing
        if (buttonIndex >= buttonsImage.Count || buttonsImage[buttonIndex] == null)
        {
            Debug.LogWarning($"Invalid or missing button image at index: {buttonIndex}. Skipping highlight animation.");
            return;
        }

        Debug.LogWarning("Highlighting button index: " + buttonIndex);
        if (buttonIndex >= 0 && buttonIndex < buttonAnimationCoroutine.Length)
        {
            buttonAnimationCoroutine[buttonIndex] = StartCoroutine(HighlightAnimation(buttonIndex));
        }
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
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
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
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            objImage.color = Color32.Lerp(startColor, targetColor, smoothLerp);
            objImage.rectTransform.localScale = Vector2.Lerp(startScale, targetScale, smoothLerp);
            yield return null;
        }

        objImage.color = targetColor;
        objImage.rectTransform.localScale = targetScale;
    }

    #region Buttons Behavior

    public void ContinueGame()
    {
        if (isInCredits) { return; }
        S_TransitionManager.instance.GoToLevel("HubWorld");
    }

    public void NewGame()
    {
        if (isInCredits) { return; }
        S_SaveManager.instance.ResetPlayerData();
        S_TransitionManager.instance.GoToLevel("HubWorld");
    }

    public void OpenCredits()
    {
        if (isInCredits) { return; }
        isInCredits = true;
        creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        if (!isInCredits) { return; }
        isInCredits = false;
        creditsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        if (isInCredits) { return; }
        Application.Quit();
        Debug.Log("Quit Game");
    }

    public void ChangeLanguage()
    {
        if (isInCredits) { return; }
        if (currentLanguage == "en")
        {
            currentLanguage = "br";
        }
        else if (currentLanguage == "br")
        {
            currentLanguage = "en";
        }

        S_SaveManager.SettingsData settingsData = S_SaveManager.instance.GetSettingsData();

        settingsData.language = currentLanguage;

        S_SaveManager.instance.SaveSettingsData(settingsData);

        S_SettingsManager.instance.RefreshLanguage(currentLanguage);

        RefreshLanguage();
    }

    public void RefreshLanguage()
    {
        if (currentLanguage == "en")
        {
            if (buttons[0] != null)
            {
                buttons[0].GetComponentInChildren<TextMeshProUGUI>().text = "Continue Game";
            }
            
            buttons[1].GetComponentInChildren<TextMeshProUGUI>().text = "New Game";
            buttons[2].GetComponentInChildren<TextMeshProUGUI>().text = "Settings";
            buttons[3].GetComponentInChildren<TextMeshProUGUI>().text = "Credits";
            buttons[4].GetComponentInChildren<TextMeshProUGUI>().text = "Quit Game";

            inputGuideText[0].text = "Navigate";
            inputGuideText[1].text = "Confirm";
            inputGuideText[2].text = "Return";

            creditsText.text = creditsContentEn;
        }
        else if (currentLanguage == "br")
        {
            if (buttons[0] != null)
            {
                buttons[0].GetComponentInChildren<TextMeshProUGUI>().text = "Continuar Jogo";
            }
            buttons[1].GetComponentInChildren<TextMeshProUGUI>().text = "Novo Jogo";
            buttons[2].GetComponentInChildren<TextMeshProUGUI>().text = "Configurações";
            buttons[3].GetComponentInChildren<TextMeshProUGUI>().text = "Créditos";
            buttons[4].GetComponentInChildren<TextMeshProUGUI>().text = "Sair do Jogo";

            inputGuideText[0].text = "Navegar";
            inputGuideText[1].text = "Confirmar";
            inputGuideText[2].text = "Voltar";

            creditsText.text = creditsContentBr;
        }

        flagIcon.sprite = currentLanguage == "en" ? flagImages[0] : flagImages[1];
    }
    public void ResumeOperation()
    {
        if (isInCredits) { return; }
        isThisMenuActive = true;
        if (mainMenuAnimationCoroutine != null)
        {
            StopCoroutine(mainMenuAnimationCoroutine);
        }
        mainMenuAnimationCoroutine = StartCoroutine(HF.SmoothRectMove(mainPanel, new Vector2(0, 0), 0.3f));
    }

    public void OpenSettingsMenu()
    {
        if (isInCredits) { return; }
        if (mainMenuAnimationCoroutine != null)
        {
            StopCoroutine(mainMenuAnimationCoroutine);
        }
        S_SettingsManager.instance.OpenSettings(this);
        mainMenuAnimationCoroutine = StartCoroutine(HF.SmoothRectMove(mainPanel, new Vector2(-800, 0), 0.3f));
        isThisMenuActive = false;
    }
    

    private bool ArePlayerDataEqual(S_SaveManager.PlayerData data1, S_SaveManager.PlayerData data2)
    {
        // Compare fields of the PlayerData struct for equality
        return data1.appleCount == data2.appleCount &&
               data1.worlds.Count == data2.worlds.Count; // Add more comparisons as needed
    }
    #endregion
}
