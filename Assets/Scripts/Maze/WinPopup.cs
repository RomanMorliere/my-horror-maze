using UnityEngine;
using UnityEngine.UI;

public class WinPopup : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private void Start()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void Show()
    {
        if (panel != null)
            panel.SetActive(true);

        Time.timeScale = 0f; // pause the game
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);

        Time.timeScale = 1f; // resume game
    }
}