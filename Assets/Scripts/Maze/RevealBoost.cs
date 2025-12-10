using UnityEngine;

public class RevealBoost : MonoBehaviour
{
    public float revealDuration = 3f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MazeGenerator generator = FindObjectOfType<MazeGenerator>();

            if (generator != null)
            {
                // This starts the logic in the MazeGenerator
                generator.StartCoroutine(generator.RevealWalls(revealDuration)); 
                
                gameObject.SetActive(false);
            }
        }
    }
}