using UnityEngine;

namespace Maze
{
    public class Shield : MonoBehaviour
    { 
        public float shieldDuration = 5f;     

        private void OnTriggerEnter(Collider other)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.StartCoroutine(player.ApplySheild(shieldDuration));
                gameObject.SetActive(false);
            }
        }
    }
}