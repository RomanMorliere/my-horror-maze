using UnityEngine;

public class MazeExit : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("YOU WON!");
            winPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
