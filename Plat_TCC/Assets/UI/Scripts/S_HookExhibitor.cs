using UnityEngine;

public class S_HookExhibitor : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera; // Reference to the main camera

    [SerializeField]
    private RectTransform uiIcon; // Reference to the UI icon (e.g., Image or Button)

    [SerializeField]
    private float interactionDistance = 5f; // Distance to show the icon

    [SerializeField]
    private Transform player; // Reference to the player

    private Transform[] hooks; // Array to store all hooks in the scene
    private Transform closestHook; // Reference to the closest hook

    [SerializeField]
    private Vector2 minScale = new Vector2(0.5f, 0.5f); // Minimum scale of the icon
    [SerializeField]
    private Vector2 maxScale = new Vector2(1f, 1f); // Maximum scale of the icon

    private CanvasGroup canvasGroup; // Reference to the CanvasGroup component

    void Start()
    {
        mainCamera = Camera.main;

        player = GameObject.FindGameObjectWithTag("Player").transform;

        GameObject[] hookObjects = GameObject.FindGameObjectsWithTag("Hook");
        hooks = new Transform[hookObjects.Length];
        for (int i = 0; i < hookObjects.Length; i++)
        {
            hooks[i] = hookObjects[i].transform;
        }

        canvasGroup = uiIcon.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup component is missing on the UI icon.");
        }
    }

    void Update()
    {
        closestHook = GetClosestHook();

        if (closestHook != null)
        {
            float distance = Vector3.Distance(player.position, closestHook.position);

            if (distance <= interactionDistance)
            {
                Vector3 screenPosition = mainCamera.WorldToScreenPoint(closestHook.position);

                // Check if the hook is in front of the camera
                if (screenPosition.z > 0)
                {
                    // Set the position of it a little higher than it should be so that it doesnt blend with the background, but if goes above the screen height, put it below

                    screenPosition.y += 50f;
                    if (screenPosition.y > Screen.height)
                    {
                        screenPosition.y = Screen.height - 50f;
                    }
                    uiIcon.position = screenPosition;
                    

                    float scaleFactor = Mathf.InverseLerp(interactionDistance, 0, distance);
                    uiIcon.localScale = Vector2.Lerp(minScale, maxScale, scaleFactor);

                    canvasGroup.alpha = Mathf.Lerp(0, 1, 1 - scaleFactor);

                    uiIcon.gameObject.SetActive(true);
                }
                else
                {
                    uiIcon.gameObject.SetActive(false);
                }
            }
            else // Too far
            {
                uiIcon.gameObject.SetActive(false);
            }
        }
        else // No hooks
        {
            uiIcon.gameObject.SetActive(false);
        }
    }

    private Transform GetClosestHook()
    {
        Transform closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform hook in hooks)
        {
            float distance = Vector3.Distance(player.position, hook.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = hook;
            }
        }

        return closest;
    }
}
