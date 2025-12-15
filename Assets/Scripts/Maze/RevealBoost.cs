using UnityEngine;

public class RevealBoost : MonoBehaviour
{
    public float revealDuration = 3f;

    public void OnTriggerEnter(Collider other) {
    if (other.CompareTag("Player")) {
        other.GetComponent<PlayerController>().AddRevealCharge();
        gameObject.SetActive(false);
    }
}
}