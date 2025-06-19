using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_CollectibleExhibitor : MonoBehaviour
{
    public static S_CollectibleExhibitor instance;

    [SerializeField]
    private float raiseSpeed = 3f;

    [SerializeField]
    private float descentSpeed = 1f;

    #region Apple
    [SerializeField]
    private RectTransform appleShadow;

    [SerializeField]
    private RectTransform appleIcon;

    [SerializeField]
    private Image appleIconImage;

    [SerializeField]
    private Image appleShadowImage;

    [SerializeField]
    private TextMeshProUGUI appleCounter;

    private Coroutine appleStretchCoroutine;

    private int appleTimer = 0;
    #endregion

    [SerializeField]
    private Sprite appleSprite;

    [SerializeField]
    private Sprite keySprite;

    [SerializeField]
    private Sprite gearSprite;

    [SerializeField]
    private Color32 appleBGColor;

    [SerializeField]
    private Color32 keyBGColor;

    [SerializeField]
    private Color32 gearBGColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateGroupPosition();
    }

    private void UpdateGroupPosition()
    {
        if (appleTimer > 0)
        {
            appleShadow.anchoredPosition = Vector2.Lerp(appleShadow.anchoredPosition, new Vector2(0, 0), Time.deltaTime * raiseSpeed);
            appleTimer--;
        }
        else
        {
            appleShadow.anchoredPosition = Vector2.Lerp(appleShadow.anchoredPosition, new Vector2(0, -900), Time.deltaTime * descentSpeed);
        }
    }

    public void UpdateCollectible(string collectibleName, int count)
    {
        switch (collectibleName)
        {
            case "Apple":

                appleIconImage.sprite = appleSprite;
                appleShadowImage.color = appleBGColor;
                break;

            case "Key":
                appleIconImage.sprite = keySprite;
                appleShadowImage.color = keyBGColor;
                break;

            case "Main":
                appleIconImage.sprite = gearSprite;
                appleShadowImage.color = gearBGColor;
                break;

        }
        appleCounter.text = count + "x";
        appleTimer = 60;
        if (appleStretchCoroutine != null) { StopCoroutine(appleStretchCoroutine); }
        appleStretchCoroutine = StartCoroutine(StretchAnimation(appleIcon, 0.15f));
    }

    private IEnumerator StretchAnimation(RectTransform icon, float duration)
    {
        icon.localScale = new Vector2(1, 1.4f);

        float lerp = 0;
        float smoothLerp = 0;
        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.unscaledDeltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            icon.localScale = Vector3.Lerp(new Vector2(1, 1.4f), new Vector2(1, 1), smoothLerp);
            yield return null;
        }
        icon.localScale = new Vector2(1, 1);
        yield return null;
    }
}
