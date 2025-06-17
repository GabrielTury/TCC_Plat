using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_CollectiblePanel : MonoBehaviour
{
    [SerializeField]
    private Image collectibleImage;

    [SerializeField]
    private TextMeshProUGUI collectibleNameText;

    [SerializeField]
    private TextMeshProUGUI collectibleCountText;

    private void Start()
    {
        // Get references to the UI components if not set in the inspector
        if (collectibleImage == null)
        {
            collectibleImage = GetComponentInChildren<Image>();
            if (collectibleImage == null)
            {
                Debug.LogError("Collectible Image component not found in children.");
            }
            collectibleCountText = GetComponentInChildren<TextMeshProUGUI>();
            if (collectibleCountText == null)
            {
                Debug.LogError("Collectible Count Text component not found in children.");
            }
            collectibleNameText = GetComponentInChildren<TextMeshProUGUI>();
            if (collectibleNameText == null)
            {
                Debug.LogError("Collectible Name Text component not found in children.");
            }
        }
    }

    public void Setup(Sprite collectibleSprite, string collectibleName, int collectibleCount)
    {
        if (collectibleImage != null)
        {
            collectibleImage.sprite = collectibleSprite;
        }
        else
        {
            Debug.LogWarning("Collectible Image is not assigned in the inspector.");
        }
        if (collectibleCountText != null)
        {
            collectibleCountText.text = "x" + collectibleCount.ToString();
        }
        else
        {
            Debug.LogWarning("Collectible Count Text is not assigned in the inspector.");
        }
        if (collectibleNameText != null)
        {
            collectibleNameText.text = collectibleName;
        }
        else
        {
            Debug.LogWarning("Collectible Name Text is not assigned in the inspector.");
        }
    }
}
