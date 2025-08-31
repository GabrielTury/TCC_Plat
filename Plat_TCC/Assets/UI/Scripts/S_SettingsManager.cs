using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using HF = S_HelperFunctions;

public class S_SettingsManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform settingsPanel;

    private IMenuCaller previousMenuScript;

    public static S_SettingsManager instance;

    private InputSystem_Actions inputs;

    public bool isOnSettingsMenu = false;

    private Coroutine settingsAnimationCoroutine;

    [SerializeField]
    private GameObject[] buttons = new GameObject[5];

    //[SerializeField]
    private CanvasGroup[] buttonsCanvas = new CanvasGroup[5];

    //[SerializeField]
    private RectTransform[] buttonsRects = new RectTransform[5];

    private TextMeshProUGUI[] buttonsText = new TextMeshProUGUI[4];

    [SerializeField]
    private int selectionIndex = 0;

    //[SerializeField]
    private Coroutine[] buttonAnimationCoroutine = new Coroutine[6];

    [SerializeField]
    private AudioMixer audioMixer;

    [SerializeField]
    private int lastButtonHighlighted = 0;

    private List<(int width, int height)> resolutions = new List<(int width, int height)>
    {
        (1280, 720),
        (1366, 768),
        (1600, 900),
        (1920, 1080),
        (2560, 1440),
        (3840, 2160)
    };

    private int resolutionIndex = 0;

    private List<int> windowTypes = new List<int>
    {
        0,
        1,
        2
    };

    private int windowTypeIndex = 0;

    private float musicVolume = 0.8f;

    private float soundVolume = 0.8f;

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
        S_SaveManager.SettingsData settingsData;
        bool exists;
        
        (settingsData, exists) = S_SaveManager.instance.LoadSettingsData();

        if (exists)
        {
            resolutionIndex = settingsData.resolutionIndex;
            windowTypeIndex = settingsData.windowTypeIndex;
            musicVolume = settingsData.musicVolume;
            soundVolume = settingsData.soundVolume;
        } else
        {
            resolutionIndex = resolutions.Count - 1; // Default to highest resolution available
            windowTypeIndex = 2; // Default to Fullscreen
            musicVolume = 0.8f; // Default to 80%
            soundVolume = 0.8f; // Default to 80%
        }

        //resolutionIndex = PlayerPrefs.GetInt("RESOLUTION_INDEX", 0);
        //windowTypeIndex = PlayerPrefs.GetInt("WINDOW_TYPE_INDEX", 0);
        //musicVolume = PlayerPrefs.GetFloat("MUSIC_VOLUME", 0.8f);
        //soundVolume = PlayerPrefs.GetFloat("SOUND_VOLUME", 0.8f);

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            ApplySavedPlayerSettings(); // Make sure the previously defined player settings are applied when the game starts
        }

        instance = this;

        for (int i = 0; i < buttons.Length; i++)
        {
            buttonsCanvas[i] = buttons[i].GetComponent<CanvasGroup>();
            buttonsRects[i] = buttons[i].GetComponent<RectTransform>();
            try
            {
                buttonsText[i] = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
                UpdateSettingSelectionDisplay(i);
            } catch { }
        }

        int distance = 200;

        for (int i = 0; i < buttons.Length; i++)
        {
            buttonsRects[i].anchoredPosition = new Vector2(-570, 500 - ((i + 1) * distance));
        }

        buttonsRects[4].anchoredPosition = new Vector2(-570, -445);

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

    private void CheckPlayerInput()
    {
        if (Time.timeSinceLevelLoad < 1.5f || isOnSettingsMenu == false || settingsPanel.anchoredPosition.x < -100)
        {
            //Debug.Log("Settings menu is not active or the panel is not visible yet.");
            return;
        }
        if (inputs.UI.Navigate.WasPressedThisFrame())
        {
            // Vertical selection

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

            // ---------------------------------------------- Horizontal selection

            if (inputs.UI.Navigate.ReadValue<Vector2>().x > 0 || inputs.UI.Navigate.ReadValue<Vector2>().x < 0)
            {
                ManageSettingSelection(inputs.UI.Navigate.ReadValue<Vector2>().x);
            }
        }

        if (inputs.UI.Submit.WasPressedThisFrame())
        {
            switch (selectionIndex)
            {
                case 0: // Resolution


                    break;

                case 1: // Window Type


                    break;

                case 2: // Music Volume


                    break;

                case 3: // Sound Volume


                    break;

                case 4: // Save

                    SaveNewSettings();
                    CloseSettings();
                    break;
            }
            //buttons[selectionIndex].onClick.Invoke();
        }

        if (inputs.UI.Cancel.WasPressedThisFrame() && isOnSettingsMenu)
        {
            CloseSettings();
        }

        if (inputs.UI.Pause.WasPressedThisFrame() && isOnSettingsMenu)
        {
            CloseSettings();
        }
    }

    public void ManageSettingSelection(float directionValue)
    {
        string direction = (directionValue > 0 ? "right" : "left");

        switch (selectionIndex)
        {
            case 0: // Resolution
                
                if (direction == "left")
                {
                    resolutionIndex = (int)HF.Wrap(resolutionIndex - 1, 0, resolutions.Count);
                }
                else if (direction == "right") 
                {
                    resolutionIndex = (int)HF.Wrap(resolutionIndex + 1, 0, resolutions.Count);
                }
                
                break;

            case 1: // Window Type

                if (direction == "left")
                {
                    windowTypeIndex = (int)HF.Wrap(windowTypeIndex - 1, 0, windowTypes.Count);
                }
                else if (direction == "right")
                {
                    windowTypeIndex = (int)HF.Wrap(windowTypeIndex + 1, 0, windowTypes.Count);
                }
                break;

            case 2: // Music Volume

                if (direction == "left")
                {
                    musicVolume = HF.WrapVolume(musicVolume, -0.1f, 0, 1);
                }
                else if (direction == "right")
                {
                    musicVolume = HF.WrapVolume(musicVolume, 0.1f, 0, 1);
                }
                musicVolume = Mathf.Round(musicVolume * 10) / 10f;
                break;

            case 3: // Sound Volume

                if (direction == "left")
                {
                    soundVolume = HF.WrapVolume(soundVolume, -0.1f, 0, 1);
                }
                else if (direction == "right")
                {
                    soundVolume = HF.WrapVolume(soundVolume, 0.1f, 0, 1);
                }
                soundVolume = Mathf.Round(soundVolume * 10) / 10f;
                break;
        }

        UpdateSettingSelectionDisplay(selectionIndex);
    }

    private void UpdateSettingSelectionDisplay(int menuIndex)
    {
        switch (menuIndex)
        {
            case 0: // Resolution

                buttonsText[menuIndex].text = $"{resolutions[resolutionIndex].width}x{resolutions[resolutionIndex].height}";
                break;

            case 1: // Window Type

                buttonsText[menuIndex].text = windowTypes[windowTypeIndex] == 0 ? "Windowed" : windowTypes[windowTypeIndex] == 1 ? "Borderless" : "Fullscreen";
                break;

            case 2: // Music Volume

                buttonsText[menuIndex].text = $"Music Volume: {(int)(musicVolume * 100)}%";
                break;

            case 3: // Sound Volume

                buttonsText[menuIndex].text = $"Sound Volume: {(int)(soundVolume * 100)}%";
                break;
        }
    }

    public void SaveAndCloseSettings()
    {
        SaveNewSettings();
        CloseSettings();
    }

    private void SaveNewSettings()
    {
        // Save the new settings to the player prefs

        Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height,
            windowTypes[windowTypeIndex] == 0 ? FullScreenMode.Windowed : windowTypes[windowTypeIndex] == 1 ? FullScreenMode.FullScreenWindow : FullScreenMode.ExclusiveFullScreen);

        S_SaveManager.SettingsData settingsData = new S_SaveManager.SettingsData
        {
            resolutionIndex = resolutionIndex,
            windowTypeIndex = windowTypeIndex,
            musicVolume = musicVolume,
            soundVolume = soundVolume
        };

        S_SaveManager.instance.SaveSettingsData(settingsData);

        //PlayerPrefs.SetInt("RESOLUTION_INDEX", resolutionIndex);
        //PlayerPrefs.SetInt("WINDOW_TYPE_INDEX", windowTypeIndex);
        //PlayerPrefs.SetFloat("MUSIC_VOLUME", musicVolume);
        //PlayerPrefs.SetFloat("SOUND_VOLUME", soundVolume);

        audioMixer.SetFloat("MusicParam", Mathf.Lerp(-80, 0, musicVolume));
        audioMixer.SetFloat("SoundParam", Mathf.Lerp(-80, 0, soundVolume));
    }

    private void ApplySavedPlayerSettings()
    {
        // Apply the saved settings to the game
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

    private IEnumerator HighlightAnimation(int buttonIndex)
    {
        CanvasGroup objImage = buttonsCanvas[buttonIndex];
        RectTransform objRect = buttonsRects[buttonIndex];
        float startAlpha = objImage.alpha;
        Vector2 startScale = objRect.localScale;
        float lerp = 0;
        float smoothLerp = 0;
        float duration = 0.2f;
        float targetAlpha = 1f;
        Vector2 targetScale = new Vector2(1f, 1f);

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.unscaledDeltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            objImage.alpha = Mathf.Lerp(startAlpha, targetAlpha, smoothLerp);
            objRect.localScale = Vector2.Lerp(startScale, targetScale, smoothLerp);
            yield return null;
        }

        objImage.alpha = targetAlpha;
        objRect.localScale = targetScale;
    }

    private IEnumerator UnHighlightAnimation(int buttonIndex)
    {
        CanvasGroup objImage = buttonsCanvas[buttonIndex];
        RectTransform objRect = buttonsRects[buttonIndex];
        float startAlpha = objImage.alpha;
        Vector2 startScale = objRect.localScale;
        float lerp = 0;
        float smoothLerp = 0;
        float duration = 0.2f;
        float targetAlpha = 0.196f;
        Vector2 targetScale = new Vector2(0.85f, 0.85f);

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.unscaledDeltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            objImage.alpha = Mathf.Lerp(startAlpha, targetAlpha, smoothLerp);
            objRect.localScale = Vector2.Lerp(startScale, targetScale, smoothLerp);
            yield return null;
        }

        objImage.alpha = targetAlpha;
        objRect.localScale = targetScale;
    }

    public void OpenSettings(IMenuCaller originMenu)
    {
        if (settingsAnimationCoroutine != null)
        {
            StopCoroutine(settingsAnimationCoroutine);
        }
        settingsAnimationCoroutine = StartCoroutine(HF.SmoothRectMove(settingsPanel, new Vector2(0, 0), 0.3f));
        previousMenuScript = originMenu;
        isOnSettingsMenu = true;
    }

    public void CloseSettings()
    {
        if (settingsAnimationCoroutine != null)
        {
            StopCoroutine(settingsAnimationCoroutine);
        }
        settingsAnimationCoroutine = StartCoroutine(HF.SmoothRectMove(settingsPanel, new Vector2(-1000, 0), 0.3f));
        previousMenuScript?.ResumeOperation();
        isOnSettingsMenu = false;
    }
}
