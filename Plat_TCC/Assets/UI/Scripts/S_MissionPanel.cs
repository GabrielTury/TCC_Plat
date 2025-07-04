using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;

public class S_MissionPanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI missionNameText;

    [SerializeField]
    private GameObject collectiblePanelPrefab;

    [SerializeField]
    private List<RectTransform> collectiblePanels = new List<RectTransform>();

    private RectTransform rectTransform;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private GameObject blockerObj;

    [SerializeField]
    private Transform collectibleHolder; // Parent for collectible panels

    private void Start()
    {
        // Get references to the UI components if not set in the inspector
        if (missionNameText == null)
        {
            missionNameText = GetComponentInChildren<TextMeshProUGUI>();
            if (missionNameText == null)
            {
                Debug.LogError("Mission Name Text component not found in children.");
            }
        }
        if (collectiblePanelPrefab == null)
        {
            Debug.LogError("Collectible Panel Prefab is not assigned in the inspector.");
        }

        rectTransform = GetComponent<RectTransform>();

        rectTransform.localScale = new Vector3(0, 0, 0);
    }

    private void FixedUpdate()
    {
        if (rectTransform.anchoredPosition.x >= 0)
        {
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, new Vector3(0f, 0f, 0f), 0.2f);
        }
        else if (rectTransform.anchoredPosition.x > -570 + 70)
        {
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, new Vector3(0.6f, 0.6f, 6f), 0.2f);
        }
        else if (rectTransform.anchoredPosition.x < -570 - 70)
        {
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, new Vector3(0.6f, 0.6f, 0.6f), 0.2f);
        }
        else
        {
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, new Vector3(1f, 1f, 1f), 0.2f);
        }

        if (canvasGroup != null)
        {
            if (rectTransform.anchoredPosition.x >= -20)
            {
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, 0.25f);
            }
            else if (rectTransform.anchoredPosition.x > -570 + 70)
            {
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0.5f, 0.25f);
            }
            else
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1f, 0.25f);
        }
        else
        {
            Debug.LogError("CanvasGroup component not found on the mission panel.");
        }
    }

    public void Setup(SO_MissionUIInfo missionInfo)
    {
        missionNameText.text = missionInfo.objectiveName;
        
        int index = 0;
        // create the collectible panels for each collectible in the mission
        foreach (var collectible in missionInfo.collectibleInfo)
        {
            GameObject collectiblePanel = Instantiate(collectiblePanelPrefab, collectibleHolder);
            S_CollectiblePanel collectiblePanelScript = collectiblePanel.GetComponent<S_CollectiblePanel>();

            if (collectiblePanelScript != null)
            {
                collectiblePanelScript.Setup(collectible.icon, collectible.collectibleName, collectible.count);
            }
            else
            {
                Debug.LogError("Collectible Panel script not found on the instantiated panel.");
            }

            RectTransform rectTransform = collectiblePanel.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                collectiblePanels.Add(rectTransform);
            }
            else
            {
                Debug.LogError("RectTransform component not found on the instantiated panel.");
            }
            index++;
        }

        // Set the position of each panel based on the index
        for (int i = 0; i < collectiblePanels.Count; i++)
        {
            RectTransform rectTransform = collectiblePanels[i];
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = new Vector2(0, 72 - i * 70);
            }
            else
            {
                Debug.LogError("RectTransform component not found on the instantiated panel.");
            }
        }

        string currentMissionCondition = missionInfo.condition;
        bool canProceed = false;

        // Debug log the condition for clarity

        Debug.Log($"Current Mission Condition: {currentMissionCondition}");

        if (string.IsNullOrEmpty(currentMissionCondition))
        {
            canProceed = true; // Default to false if condition is not set
        }
        else
        {
            if (PlayerPrefs.GetString(currentMissionCondition, "false") == "true")
            {
                canProceed = true;
            }
            else
            {
                canProceed = false;
            }
        }

        // Debug log the result of the condition check and the mission condition

        Debug.Log($"Can Proceed: {canProceed} for condition: {currentMissionCondition}");

        if (canProceed == true)
        {
            Destroy(blockerObj);
        }
    }
}
