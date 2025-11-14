using UnityEngine;

public class MazeExit : MonoBehaviour
{
    [SerializeField] private WinPopup winPopup;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null && winPopup != null)
        {
            winPopup.Show();
        }
    }
}