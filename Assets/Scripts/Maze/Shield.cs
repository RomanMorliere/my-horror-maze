using UnityEngine;

namespace Maze
{
    public class Shield : MonoBehaviour
    { 
        public float shieldDuration = 5f;     

        public void OnTriggerEnter(Collider other) {
    if (other.CompareTag("Player")) {
        other.GetComponent<PlayerController>().AddShieldCharge();
        gameObject.SetActive(false);
    }
}
    }
}