using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_DialogSystem : MonoBehaviour
{
    public static S_DialogSystem instance;

    [SerializeField]
    private TextMeshProUGUI speakerNameText;

    [SerializeField]
    private Image backgroundImage;

    [SerializeField]
    private RectTransform backgroundRectTransform;

    [SerializeField]
    private Image speakerImage;

    [SerializeField]
    private TextMeshProUGUI dialogText;

    [SerializeField]
    private Canvas dialogCanvas;

    [System.Serializable]
    public struct Sentence
    {
        public Sprite speakerSprite;
        public string speakerName;
        [TextArea(3, 3)]
        public string sentenceText;
    }

    private float targetRaisedHeight = -350f;

    private float targetLoweredHeight = -700f;

    [SerializeField]
    private float movementSpeed = 0.5f;

    [SerializeField]
    private float typingSpeed = 0.02f; // Time delay between each character

    [SerializeField]
    private float displayDuration = 2f; // Time to display the full sentence before lowering the box

    [SerializeField]
    private float boopDuration = 0.1f; // Duration of the boop animation

    private Coroutine typingCoroutine = null;

    private Coroutine movementCoroutine = null;

    private Coroutine boopingCoroutine = null;

    
    public bool IsDialogActive()
    {
        return typingCoroutine != null || movementCoroutine != null || boopingCoroutine != null;
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        backgroundRectTransform.anchoredPosition = new Vector2(backgroundRectTransform.anchoredPosition.x, targetLoweredHeight);
    }

    public void RaiseSingleSentence(Sentence sentence)
    {
        int charCount = sentence.sentenceText.Length;

        dialogText.text = "";
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        typingCoroutine = StartCoroutine(TypeSentence(sentence.sentenceText, charCount));
        speakerImage.sprite = sentence.speakerSprite;
        speakerNameText.text = sentence.speakerName;
        RaiseBox();
        BoopBox();
    }

    public void RaiseMultipleSentence(Sentence[] sentences)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        StartCoroutine(DisplaySentencesSequentially(sentences));
    }

    private IEnumerator DisplaySentencesSequentially(Sentence[] sentences)
    {
        foreach (Sentence s in sentences)
        {
            int charCount = s.sentenceText.Length;
            dialogText.text = "";
            speakerImage.sprite = s.speakerSprite;
            speakerNameText.text = s.speakerName;
            RaiseBox();
            BoopBox();
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
            typingCoroutine = StartCoroutine(TypeSentence(s.sentenceText, charCount));
            yield return typingCoroutine;
            typingCoroutine = null;
        }
        movementCoroutine = null;
    }

    public void RaisePredefinedDialog(SO_PredefinedDialog dialog)
    {
        StartCoroutine(DisplaySentencesSequentially(dialog.sentences));
    }

    private void RaiseBox()
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }
        movementCoroutine = StartCoroutine(MovementAnimation(targetRaisedHeight));
    }

    private void LowerBox()
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }
        movementCoroutine = StartCoroutine(MovementAnimation(targetLoweredHeight));
    }

    private void BoopBox()
    {
        if (boopingCoroutine != null)
        {
            StopCoroutine(boopingCoroutine);
        }
        boopingCoroutine = StartCoroutine(BoopingAnimation());
    }

    public void ResetAndStopDialogueInstantly()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }
        if (boopingCoroutine != null)
        {
            StopCoroutine(boopingCoroutine);
            boopingCoroutine = null;
        }
        dialogText.text = "";
        backgroundRectTransform.anchoredPosition = new Vector2(backgroundRectTransform.anchoredPosition.x, targetLoweredHeight);
    }

    private IEnumerator TypeSentence(string sentence, int charCount)
    {
        dialogText.text = "";
        for (int i = 0; i < charCount; i++)
        {
            dialogText.text += sentence[i];
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(displayDuration);

        LowerBox();
        typingCoroutine = null;
    }

    private IEnumerator BoopingAnimation()
    {
        Vector3 originalScale = new Vector3(1f, 1f, 1f);
        Vector3 targetScale = originalScale * 1.1f;
        float elapsedTime = 0f;
        float duration = boopDuration;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            t = 1f - (1f - t) * (1f - t);
            backgroundRectTransform.localScale = Vector3.LerpUnclamped(originalScale, targetScale, t);
            yield return null;
        }
        backgroundRectTransform.localScale = targetScale;
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            t = 1f - (1f - t) * (1f - t);
            backgroundRectTransform.localScale = Vector3.LerpUnclamped(targetScale, originalScale, t);
            yield return null;
        }
        backgroundRectTransform.localScale = originalScale;
        boopingCoroutine = null;
    }

    private IEnumerator MovementAnimation(float targetY)
    {
        RectTransform rectTransform = backgroundRectTransform;
        float startY = rectTransform.anchoredPosition.y;
        float elapsedTime = 0f;
        float duration = movementSpeed;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            t = 1f - (1f - t) * (1f - t);
            float newY = Mathf.LerpUnclamped(startY, targetY, t);
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, newY);
            yield return null;
        }
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, targetY);
        movementCoroutine = null;
    }
}
