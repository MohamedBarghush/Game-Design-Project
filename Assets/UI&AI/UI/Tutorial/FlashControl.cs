using UnityEngine;
using System.Collections;
using System;

public class FlashControl : MonoBehaviour
{
    public static FlashControl instance;
    private void Awake()
    {
        instance = this;
    }

    [SerializeField] private Transform parent;          // Optional parent for the UI instance
    [SerializeField] private float flashDuration = 0.5f;

    public void FlashUI(GameObject uiPrefab, TutorialSaves check = null)
    {
        if (check != null) {
            if (parent.childCount > 0)
            {
                Destroy(parent.GetChild(0).gameObject);
            }
        }
        GameObject uiInstance = Instantiate(uiPrefab, parent);
        CanvasGroup canvasGroup = uiInstance.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            Debug.LogError("UI prefab must have a CanvasGroup component!");
            Destroy(uiInstance);
            return;
        }

        StartCoroutine(FlashCoroutine(canvasGroup, uiInstance));
    }

    private IEnumerator FlashCoroutine(CanvasGroup canvasGroup, GameObject uiObject)
    {
        int flashCount = 0;

        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, flashDuration));

        while (flashCount < 2)
        {
            // Fade in
            yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0.2f, flashDuration));
            // Fade out
            yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 0.2f, 1f, flashDuration));
            flashCount++;
        }

        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, flashDuration));

        Destroy(uiObject);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        cg.alpha = startAlpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }

        cg.alpha = endAlpha;
    }
}
