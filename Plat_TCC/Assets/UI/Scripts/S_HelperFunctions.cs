using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UI;

public static class S_HelperFunctions
{
    public static IEnumerator SmoothMove(Image objImage, Vector2 targetPosition, float duration)
    {
        Vector2 startPosition = objImage.rectTransform.anchoredPosition;
        float lerp = 0;
        float smoothLerp = 0;

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            objImage.rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, smoothLerp);
            yield return null;
        }

        objImage.rectTransform.anchoredPosition = targetPosition;
    }

    public static IEnumerator SmoothRectMove(RectTransform objImage, Vector2 targetPosition, float duration)
    {
        Vector2 startPosition = objImage.anchoredPosition;
        float lerp = 0;
        float smoothLerp = 0;

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            objImage.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, smoothLerp);
            yield return null;
        }

        objImage.anchoredPosition = targetPosition;
    }

    public static IEnumerator SmoothRawMove(RawImage objImage, Vector2 targetPosition, float duration)
    {
        Vector2 startPosition = objImage.rectTransform.anchoredPosition;
        float lerp = 0;
        float smoothLerp = 0;

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            objImage.rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, smoothLerp);
            yield return null;
        }

        objImage.rectTransform.anchoredPosition = targetPosition;
    }

    public static IEnumerator SmoothResize(Image objImage, Vector2 targetScaleMin, Vector2 targetScaleMax, float duration)
    {
        Vector2 startScaleMin = objImage.rectTransform.anchorMin;
        Vector2 startScaleMax = objImage.rectTransform.anchorMax;
        float lerp = 0;
        float smoothLerp = 0;

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            objImage.rectTransform.anchorMin = Vector2.Lerp(startScaleMin, targetScaleMin, smoothLerp);
            objImage.rectTransform.anchorMax = Vector2.Lerp(startScaleMax, targetScaleMax, smoothLerp);
            yield return null;
        }

        objImage.rectTransform.anchorMin = targetScaleMin;
        objImage.rectTransform.anchorMax = targetScaleMax;
    }

    public static IEnumerator SmoothResize2(Image objImage, Vector2 targetScale, float duration)
    {
        Vector2 startScale = objImage.rectTransform.sizeDelta;
        float lerp = 0;
        float smoothLerp = 0;

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            objImage.rectTransform.sizeDelta = Vector2.Lerp(startScale, targetScale, smoothLerp);
            yield return null;
        }

        objImage.rectTransform.sizeDelta = targetScale;
    }

    public static IEnumerator SmoothMoveCamera(Camera objCamera, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = objCamera.transform.position;
        float lerp = 0;
        float smoothLerp = 0;
        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            objCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, smoothLerp);
            yield return null;
        }
        objCamera.transform.position = targetPosition;
    }

    public static IEnumerator SmoothRotateCamera(Camera objCamera, Vector3 targetRotation, float duration)
    {
        Vector3 startRotation = objCamera.transform.eulerAngles;
        float lerp = 0;
        float smoothLerp = 0;
        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            objCamera.transform.eulerAngles = Vector3.Lerp(startRotation, targetRotation, smoothLerp);
            yield return null;
        }
        objCamera.transform.eulerAngles = targetRotation;
    }

    public static IEnumerator SmoothScale(Image objImage, Vector2 targetScale, float duration, bool deleteOnEnd = false)
    {
        Vector2 startScale = objImage.rectTransform.localScale;
        float lerp = 0;
        float smoothLerp = 0;

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            objImage.rectTransform.localScale = Vector2.Lerp(startScale, targetScale, smoothLerp);
            yield return null;
        }

        objImage.rectTransform.localScale = targetScale;

        if (deleteOnEnd == true)
        {
            GameObject.Destroy(objImage.gameObject);
        }
    }

    public static IEnumerator SmoothFade(Image objImage, Color32 targetColor, float duration, bool disableOnEnd = false)
    {
        Color32 startColor = objImage.color;
        float lerp = 0;
        float smoothLerp = 0;

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            objImage.color = Color32.Lerp(startColor, targetColor, smoothLerp);
            yield return null;
        }

        objImage.color = targetColor;
        if (disableOnEnd == true)
        {
            objImage.gameObject.SetActive(false);
        }
    }

    public static IEnumerator SmoothFadeText(TextMeshProUGUI textObj, Color32 targetColor, float duration, bool disableOnEnd = false)
    {
        Color32 startColor = textObj.color;
        float lerp = 0;
        float smoothLerp = 0;

        while (lerp < 1 && duration > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / duration);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            textObj.color = Color32.Lerp(startColor, targetColor, smoothLerp);
            yield return null;
        }

        textObj.color = targetColor;
        if (disableOnEnd == true)
        {
            textObj.gameObject.SetActive(false);
        }
    }

    public static GameObject GetChildWithName(GameObject obj, string name)
    {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);
        if (childTrans != null)
        {
            return childTrans.gameObject;
        }
        else
        {
            return null;
        }
    }

    public static void Move<T>(this List<T> list, T item, int newIndex)
    {
        if (item != null)
        {
            var oldIndex = list.IndexOf(item);
            if (oldIndex > -1)
            {
                list.RemoveAt(oldIndex);

                if (newIndex > oldIndex) newIndex--;

                list.Insert(newIndex, item);
            }
        }
    }

    public static Texture2D DupeTexture(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }

    public static (int width, int height) GetScaledDimensions(int originalWidth, int originalHeight, int maxWidth, int maxHeight)
    {
        float aspectRatio = (float)originalWidth / originalHeight;
        int newWidth = maxWidth;
        int newHeight = Mathf.RoundToInt(maxWidth / aspectRatio);

        if (newHeight > maxHeight)
        {
            newHeight = maxHeight;
            newWidth = Mathf.RoundToInt(maxHeight * aspectRatio);
        }

        return (newWidth, newHeight);
    }

    public static Vector2 SizeToParent(this RawImage image, float padding = 0)
    {
        var parent = image.transform.parent.GetComponentInParent<RectTransform>();
        var imageTransform = image.GetComponent<RectTransform>();
        if (!parent) { return imageTransform.sizeDelta; } //if we don't have a parent, just return our current width;
        padding = 1 - padding;
        float w = 0, h = 0;
        float ratio = image.texture.width / (float)image.texture.height;
        var bounds = new Rect(0, 0, parent.rect.width, parent.rect.height);
        if (Mathf.RoundToInt(imageTransform.eulerAngles.z) % 180 == 90)
        {
            //Invert the bounds if the image is rotated
            bounds.size = new Vector2(bounds.height, bounds.width);
        }
        //Size by height first
        h = bounds.height * padding;
        w = h * ratio;
        if (w > bounds.width * padding)
        { //If it doesn't fit, fallback to width;
            w = bounds.width * padding;
            h = w / ratio;
        }
        imageTransform.sizeDelta = new Vector2(w, h);
        return imageTransform.sizeDelta;
    }
}
