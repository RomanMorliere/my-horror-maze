using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RulePopup : MonoBehaviour
{
    public GameObject rulesPanel;
    private CanvasGroup rulesGroup;

    public float fadeDuration = 0.5f;
    public string gameSceneName = "GameScene";

    private void Awake()
    {
        rulesGroup = rulesPanel.GetComponent<CanvasGroup>();

        // Ensure it starts hidden
        rulesGroup.alpha = 0;
        rulesGroup.interactable = false;
        rulesGroup.blocksRaycasts = false;
    }

    public void OpenRules()
    {
        StartCoroutine(FadeCanvasGroup(rulesGroup, 0f, 1f));
    }

    public void CloseRules()
    {
        StartCoroutine(FadeCanvasGroup(rulesGroup, 1f, 0f));
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end)
    {
        float elapsed = 0f;

        cg.blocksRaycasts = true;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / fadeDuration);
            yield return null;
        }

        cg.alpha = end;

        cg.interactable = (end == 1);
        cg.blocksRaycasts = (end == 1);
    }
}
