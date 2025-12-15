using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.AddSpeedCharge(); // ⭐ Add charge to inventory
            Debug.Log("🏎️ Speed Boost collected!");
            gameObject.SetActive(false); // Remove from maze
        }
    }
}