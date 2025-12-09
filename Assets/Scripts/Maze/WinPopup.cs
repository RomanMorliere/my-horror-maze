using UnityEngine;
using UnityEngine.SceneManagement;

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

        Time.timeScale = 0f; // freeze gameplay
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
