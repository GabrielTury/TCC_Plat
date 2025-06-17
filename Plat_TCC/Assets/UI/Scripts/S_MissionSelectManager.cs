using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_MissionSelectManager : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private SO_MissionUIInfo[] missionInfos;

    [SerializeField]
    private GameObject panelPrefab;

    [SerializeField]
    private List<RectTransform> missionPanels = new List<RectTransform>();

    private int mainPosX = -570; // Main position X for the mission panels

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasGroup.alpha = 0f; // Set the initial alpha to 0 for fade-in effect
        FadeIn(1);

        int index = 0;

        // for each mission info, instantiate a panel and set it up

        foreach (SO_MissionUIInfo missionInfo in missionInfos)
        {
            GameObject panel = Instantiate(panelPrefab, transform);
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

    public void Setup(SO_MissionUIInfo[] missionInfos)
    {
        //Debug.LogWarning(missionInfos.Length + " missions set up in S_MissionSelectManager.");
        this.missionInfos = missionInfos;
    }
}
