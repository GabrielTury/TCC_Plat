using UnityEngine;

public class S_ClearDemoManager : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //string miss1 = PlayerPrefs.GetString("Mission1-1Completed", "false");
        //string miss2 = PlayerPrefs.GetString("Mission1-2Completed", "false");
        //string miss3 = PlayerPrefs.GetString("Mission1-3Completed", "false");

        bool missB1 = S_SaveManager.instance.GetMissionStatus(1, 1);
        bool missB2 = S_SaveManager.instance.GetMissionStatus(1, 2);
        bool missB3 = S_SaveManager.instance.GetMissionStatus(1, 3);

        if (missB1 == true && missB2 == true && missB3 == true)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f; // Set the alpha to fully opaque
                canvasGroup.interactable = true; // Make it interactable
                canvasGroup.blocksRaycasts = true; // Allow it to block raycasts
                Cursor.lockState = CursorLockMode.None; // Unlock the cursor
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
        Cursor.lockState = CursorLockMode.Locked; // Unlock the cursor
        Destroy(gameObject);
    }

    public void QuitToTitle()
    {
        S_TransitionManager.instance.GoToLevel("MainMenu");
    }
}
