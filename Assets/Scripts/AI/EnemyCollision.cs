using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("⚠️ Enemy caught player!");
            GameManager.Instance.PlayerCaught();
        }
    }
}
