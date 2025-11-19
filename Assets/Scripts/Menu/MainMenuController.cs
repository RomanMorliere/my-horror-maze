using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        // Loads next scene (game scene)
        SceneManager.LoadScene("SampleScene"); // Change to your actual scene name
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void OpenAbout()
    {
        // Show About popup or go to a new scene
        SceneManager.LoadScene("AboutScene"); // OPTIONAL: replace with UI panel toggle later
    }
}
