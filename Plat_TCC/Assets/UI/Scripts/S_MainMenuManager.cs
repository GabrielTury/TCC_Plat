using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
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
    private Button[] buttons = new Button[4];

    //[SerializeField]
    private Image[] buttonsImage = new Image[4];

    [SerializeField]
    private int selectionIndex = 0;

    //[SerializeField]
    private Coroutine[] buttonAnimationCoroutine = new Coroutine[4];

    [SerializeField]
    private int lastButtonHighlighted = 0;

    [SerializeField]
    private InputSystem_Actions inputs;

    private bool isThisMenuActive = true;

    private Coroutine mainMenuAnimationCoroutine;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeSceneObjects();
        BeginSceneAnimation();
        for (int i = 0; i < buttons.Length; i++)
        {
            HighlightButton(i);
        }
        HighlightButton(selectionIndex);
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

    private void InitializeSceneObjects()
    {
        mainCamera.transform.position = new Vector3(0, 6, 35);
        mainCamera.transform.eulerAngles = new Vector3(-70, 0, 0);

        shadowScroller.rectTransform.anchoredPosition = new Vector2(-1615, 0);

        gameLogo.rectTransform.anchoredPosition = new Vector2(-570, 750);
        gameLogo.rectTransform.localScale = new Vector2(0, 0);

        int distance = 140;
        for (int i = 0; i < buttons.Length; i++)
        {
            buttonsImage[i] = buttons[i].GetComponent<Image>();
        }
        for (int i = 0; i < buttons.Length; i++)
        {
            buttonsImage[i].rectTransform.anchoredPosition = new Vector2(-570, -800 - ((i + 1) * distance));
        }
        
    }
    
    private void BeginSceneAnimation()
    {
        StartCoroutine(HF.SmoothMoveCamera(mainCamera, new Vector3(-1, 1, 45), 5));
        StartCoroutine(HF.SmoothRotateCamera(mainCamera, new Vector3(30+360, 50, 0), 5));

        // Move the shadow scroller to the right
        StartCoroutine(HF.SmoothRawMove(shadowScroller, new Vector2(-750, 0), 1.5f));

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
        if (Time.timeSinceLevelLoad < 1.5f || isThisMenuActive == false || mainPanel.anchoredPosition.x < -100)
        {
            return;
        }
        if (inputs.UI.Navigate.WasPressedThisFrame())
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
        
        if (inputs.UI.Submit.WasPressedThisFrame())
        {
            buttons[selectionIndex].onClick.Invoke();
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

    public void ResumeOperation()
    {
        isThisMenuActive = true;
        if (mainMenuAnimationCoroutine != null)
        {
            StopCoroutine(mainMenuAnimationCoroutine);
        }
        mainMenuAnimationCoroutine = StartCoroutine(HF.SmoothRectMove(mainPanel, new Vector2(0, 0), 0.3f));
    }

    public void OpenSettingsMenu()
    {
        if (mainMenuAnimationCoroutine != null)
        {
            StopCoroutine(mainMenuAnimationCoroutine);
        }
        S_SettingsManager.instance.OpenSettings(this);
        mainMenuAnimationCoroutine = StartCoroutine(HF.SmoothRectMove(mainPanel, new Vector2(-800, 0), 0.3f));
        isThisMenuActive = false;
    }
    #endregion
}
