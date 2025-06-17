using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class S_MissionPanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI missionNameText;

    [SerializeField]
    private GameObject collectiblePanelPrefab;

    [SerializeField]
    private List<RectTransform> collectiblePanels = new List<RectTransform>();

    private RectTransform rectTransform;

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
        if (rectTransform.anchoredPosition.x > 0)
        {
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, new Vector3(0f, 0f, 0f), 0.1f);
        }
        else if (rectTransform.anchoredPosition.x > -570)
        {
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, new Vector3(0.6f, 0.6f, 6f), 0.1f);
        }
        else
        {
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, new Vector3(1f, 1f, 1f), 0.1f);
        }
    }

    public void Setup(SO_MissionUIInfo missionInfo)
    {
        missionNameText.text = missionInfo.objectiveName;
        
        int index = 0;
        // create the collectible panels for each collectible in the mission
        foreach (var collectible in missionInfo.collectibleInfo)
        {
            GameObject collectiblePanel = Instantiate(collectiblePanelPrefab, transform);
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


    }
}
