using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Windows;

public class S_MissionSelectManager : MonoBehaviour
{
    private InputSystem_Actions inputs;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private SO_MissionUIInfo[] missionInfos;

    private string levelName;

    [SerializeField]
    private GameObject panelPrefab;

    [SerializeField]
    private List<RectTransform> missionPanels = new List<RectTransform>();

    private int mainPosX = -570; // Main position X for the mission panels

    private int currentIndex = 0;

    private bool canInteract = true; // Flag to control interaction

    [SerializeField]
    private Transform missionPanelsHolder; // Reference to the holder for mission panels

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
        Cursor.lockState = CursorLockMode.None;

        canvasGroup.alpha = 0f; // Set the initial alpha to 0 for fade-in effect
        FadeIn(1);

        int index = 0;

        // for each mission info, instantiate a panel and set it up

        foreach (SO_MissionUIInfo missionInfo in missionInfos)
        {
            GameObject panel = Instantiate(panelPrefab, missionPanelsHolder);
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            missionPanels.Add(panelRect);

            // Set up the panel with the mission info

            S_MissionPanel missionPanel = panel.GetComponent<S_MissionPanel>();
            if (missionPanel != null)
            {
                missionPanel.Setup(missionInfo);
            }
            index++;
        }

        // Set the position of each panel based on the index
        for (int i = 0; i < missionPanels.Count; i++)
        {
            RectTransform rectTransform = missionPanels[i];
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = new Vector2(mainPosX + (500 * i), -40);
            }
            else
            {
                Debug.LogError("RectTransform component not found on the instantiated panel.");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // update the position of the panels based on the current index
        for (int i = 0; i < missionPanels.Count; i++)
        {
            RectTransform rectTransform = missionPanels[i];
            if (rectTransform != null)
            {
                // Calculate the new position based on the current index
                //float newXPosition = mainPosX + (500 * (i - currentIndex));
                //rectTransform.anchoredPosition = new Vector2(newXPosition, -40);

                // Calculate the new position based on the current index with a smooth transition

                float targetXPosition = mainPosX + (500 * (i - currentIndex));
                float currentXPosition = rectTransform.anchoredPosition.x;
                float newXPosition = Mathf.Lerp(currentXPosition, targetXPosition, Time.deltaTime * 9f); // Adjust the speed as needed
                rectTransform.anchoredPosition = new Vector2(newXPosition, -40);

            }
            else
            {
                Debug.LogError("RectTransform component not found on the instantiated panel.");
            }
        }

        // Check if can interact with the mission panels
        if (canInteract)
        {
            // Handle input for navigating through the mission panels
            if (inputs.UI.Navigate.WasPressedThisFrame())
            {
                if (inputs.UI.Navigate.ReadValue<Vector2>().x > 0.5f && currentIndex < missionPanels.Count - 1)
                {
                    currentIndex++;
                    //Debug.Log("Current Index: " + currentIndex);
                }
                else if (inputs.UI.Navigate.ReadValue<Vector2>().x < -0.5f && currentIndex > 0)
                {
                    currentIndex--;
                    //Debug.Log("Current Index: " + currentIndex);
                }
            }

            if (inputs.UI.Submit.WasPressedThisFrame())
            {
                TryStartMission(); // Check if the player is trying to start the mission
            }
            
        }
    }

    public void TryStartMission()
    {
        string currentMissionCondition = missionInfos[currentIndex].condition;
        bool canProceed = false;

        // Debug log the condition for clarity

        Debug.Log($"Current Mission Condition: {currentMissionCondition}");

        if (string.IsNullOrEmpty(currentMissionCondition))
        {
            canProceed = true; // Default to false if condition is not set
        }
        else
        {
            // Convert the condition string into a workable format for the new save system
            // Get the first number in the condition string as the world number
            // Get the second number in the condition string as the mission number

            string[] conditionParts = currentMissionCondition.Split('-');
            int worldNumber = 0, missionNumber = 0;
            bool conditionMet = false;

            if (conditionParts.Length >= 2)
            {
                Regex numRegex = new Regex(@"\d+");
                Match worldMatch = numRegex.Match(conditionParts[0]);
                Match missionMatch = numRegex.Match(conditionParts[1]);

                if (worldMatch.Success && missionMatch.Success &&
                    int.TryParse(worldMatch.Value, out worldNumber) &&
                    int.TryParse(missionMatch.Value, out missionNumber))
                {
                    conditionMet = S_SaveManager.instance.GetMissionStatus(worldNumber, missionNumber);
                }
                else
                {
                    Debug.LogError($"Invalid mission condition format: {currentMissionCondition}");
                }
            }
            else
            {
                Debug.LogError($"Invalid mission condition format: {currentMissionCondition}");
            }
        }
        if (canInteract && canProceed == true)
        {
            // Start the mission with the current index
            Cursor.lockState = CursorLockMode.Locked;
            Debug.Log("Next level: " + levelName + " with mission index: " + currentIndex);
            S_TransitionManager.instance.GoToLevelWithMission(levelName, currentIndex);
            canInteract = false; // Disable interaction to prevent multiple submissions
        }
    }

    public void MoveDirection(int direction)
    {
        if (direction < 0)
        {
            if (currentIndex > 0)
            {
                currentIndex--;
            }
        }
        else if (direction > 0)
        {
            if (currentIndex < missionPanels.Count - 1)
            {
                currentIndex++;
            }
        }
    }

    // coroutine to fade in the canvas group
    public void FadeIn(float duration)
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1f, duration));
    }
    public void FadeOut(float duration)
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0f, duration));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            yield return null; // Wait for the next frame
        }
        canvasGroup.alpha = endAlpha; // Ensure the final alpha is set
    }

    public void Setup(SO_MissionUIInfo[] missionInfos, string levelName)
    {
        //Debug.LogWarning(missionInfos.Length + " missions set up in S_MissionSelectManager.");
        this.missionInfos = missionInfos;
        this.levelName = levelName;
    }
}
