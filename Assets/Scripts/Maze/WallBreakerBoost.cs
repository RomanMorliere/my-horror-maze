using UnityEngine;

public class WallBreakerBoost : MonoBehaviour
{
    public int breakCharges = 3; // How many walls they can break
public void OnTriggerEnter(Collider other) {
    if (other.CompareTag("Player")) {
        other.GetComponent<PlayerController>().AddWallBreakerCharges(breakCharges);
        gameObject.SetActive(false);
    }
}
}