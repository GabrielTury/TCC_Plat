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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeSceneObjects();
        BeginSceneAnimation();
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
            buttons[i].GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(-570, -800 - ((i + 1) * distance));
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
            StartCoroutine(HF.SmoothMove(buttons[i].GetComponent<Image>(), new Vector2(-570, 0 - (i * distance)), timeDiff[i]));
        }
    }

    private void ScrollBackgroundShadow(float speed = 1)
    {
        shadowScroller.uvRect = new Rect(0, shadowScroller.uvRect.y - speed, 1, 1);
    }

    #region Buttons Behavior

    public void ResumeOperation()
    {
        StartCoroutine(HF.SmoothRectMove(mainPanel, new Vector2(0, 0), 0.5f));
    }

    public void OpenSettingsMenu()
    {
        S_SettingsManager.instance.OpenSettings(this);
        StartCoroutine(HF.SmoothRectMove(mainPanel, new Vector2(-800, 0), 0.5f));
    }
    #endregion
}
