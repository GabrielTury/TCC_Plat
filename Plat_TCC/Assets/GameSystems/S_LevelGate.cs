using UnityEngine;

public class S_LevelGate : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;

    private CanvasGroup canvasGroup;

    private RectTransform canvasRect;

    //[Header("Level Gate")]
    [SerializeField]
    private float targetHeight = 3.5f;

    [SerializeField]
    private float animationSpeed = 0.1f;

    private bool playerIsInRegion = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasRect = canvas.GetComponent<RectTransform>();
        canvasGroup = canvas.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (playerIsInRegion)
        {
            Vector3 newPosition = canvasRect.anchoredPosition;
            newPosition.y = Mathf.Lerp(newPosition.y, targetHeight, animationSpeed);
            canvasRect.anchoredPosition = newPosition;

            if (canvasRect.localScale.x < 1)
            {
                canvasRect.localScale = new Vector3(Mathf.Lerp(canvasRect.localScale.x, 1, animationSpeed), canvasRect.localScale.y, 1);
            }

            if (canvasGroup.alpha < 0.9f)
            {
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0.7f, animationSpeed);
            }

        }
        else
        {
            Vector3 newPosition = canvasRect.anchoredPosition;
            newPosition.y = Mathf.Lerp(newPosition.y, 0, animationSpeed);
            canvasRect.anchoredPosition = newPosition;

            if (canvasRect.localScale.x > 0)
            {
                canvasRect.localScale = new Vector3(Mathf.Lerp(canvasRect.localScale.x, 0, animationSpeed), canvasRect.localScale.y, 1);
            }

            if (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, animationSpeed);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.CompareTag("Player"))
        {
            playerIsInRegion = true;
            //Debug.Log("is in region");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.CompareTag("Player"))
        {
            playerIsInRegion = false;
            //Debug.Log("is out of region");
        }
    }
}
