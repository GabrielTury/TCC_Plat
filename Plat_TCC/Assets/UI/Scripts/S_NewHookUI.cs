using UnityEngine;
using UnityEngine.UI;

public class S_NewHookUI : MonoBehaviour
{
    private Color32 idleColor = new Color32(125, 125, 125, 150);
    private Color32 activeColor = new Color32(255, 5, 5, 200);

    private Image hookImage;

    private RectTransform hookRect;

    private Transform playerTransform;

    private CanvasGroup canvasGroup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find the player transform

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        hookImage = transform.GetChild(0).GetComponent<Image>();
        hookRect = hookImage.GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        // check if the player is near this hook

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer < 13f) // Assuming 5 units is the "near" distance
        {
            // Player is near, set to active color
            hookImage.color = Color32.Lerp(hookImage.color, activeColor, Time.deltaTime * 8f);
        }
        else
        {
            // Player is not near, set to idle color
            hookImage.color = Color32.Lerp(hookImage.color, idleColor, Time.deltaTime * 8f);
        }

        // if the player is very far, lerp canvas group alpha to 0

        if (distanceToPlayer > 30f) // Assuming 10 units is the "far" distance
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, Time.deltaTime * 5f);
        }
        else
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1f, Time.deltaTime * 5f);
        }

        // make hook image always face camera

        //transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        // hook, not this object (which is a canvas)

        // make hook image always face camera

        Vector3 directionToCamera = Camera.main.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToCamera);
        hookImage.transform.rotation = lookRotation;
    }
}
