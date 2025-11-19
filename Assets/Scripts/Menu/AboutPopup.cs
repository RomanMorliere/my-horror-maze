using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class AboutPopup : MonoBehaviour
{
    public GameObject aboutPanel;
    public float fadeSpeed = 2f;

    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = aboutPanel.AddComponent<CanvasGroup>();
        aboutPanel.SetActive(false);
        canvasGroup.alpha = 0;
    }

    public void ShowPopup()
    {
        aboutPanel.SetActive(true);
        StartCoroutine(FadeIn());
    }

    public void HidePopup()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeIn()
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        aboutPanel.SetActive(false);
    }
}
