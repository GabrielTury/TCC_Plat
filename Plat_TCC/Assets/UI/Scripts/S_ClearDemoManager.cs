using UnityEngine;

public class S_ClearDemoManager : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string miss1 = PlayerPrefs.GetString("Mission1-1Completed", "false");
        string miss2 = PlayerPrefs.GetString("Mission1-2Completed", "false");
        string miss3 = PlayerPrefs.GetString("Mission1-3Completed", "false");

        if (miss1 == "true" && miss2 == "true" && miss3 == "true")
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f; // Set the alpha to fully opaque
                canvasGroup.interactable = true; // Make it interactable
                canvasGroup.blocksRaycasts = true; // Allow it to block raycasts
            }
            else
            {
                Debug.LogError("CanvasGroup component not found on this GameObject.");
            }
        }
        else
        {
            Destroy(gameObject); // Destroy the GameObject if conditions are not met
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void KeepPlaying()
    {
        Destroy(gameObject);
    }

    public void QuitToTitle()
    {
        S_TransitionManager.instance.GoToLevel("MainMenu");
    }
}
