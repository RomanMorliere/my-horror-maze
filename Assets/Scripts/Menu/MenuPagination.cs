using UnityEngine;

public class MenuPagination : MonoBehaviour
{
    public GameObject[] pages; // Drag Page1, Page2, Page3 here in order
    private int currentIndex = 0;

    public void ShowNextPage()
    {
        // Hide the current page
        pages[currentIndex].SetActive(false);

        // Move to next index (loops back to 0 if at the end)
        currentIndex = (currentIndex + 1) % pages.Length;

        // Show the new page
        pages[currentIndex].SetActive(true);
    }

    // Call this when the "About" button is first clicked to reset to Page 1
    public void ResetMenu()
    {
        currentIndex = 0;
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == 0);
        }
    }

    // ⭐ ADD THIS FUNCTION
    public void CloseAboutMenu()
    {
        // This turns off the entire About Menu object
        this.gameObject.SetActive(false);
    }
}