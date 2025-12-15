using UnityEngine;

public class ProximitySound : MonoBehaviour
{
    public AudioSource proximitySource; 
    public Transform enemy;            
    public float detectionRadius = 15f; 

    void Update()
    {
        if (enemy == null || proximitySource == null) return;

        float distance = Vector3.Distance(transform.position, enemy.position);

        if (distance < detectionRadius)
        {
            if (!proximitySource.isPlaying) proximitySource.Play();
            
            // This math makes it louder as you get closer
            float volume = 1 - (distance / detectionRadius);
            proximitySource.volume = Mathf.Clamp01(volume);
        }
        else
        {
            if (proximitySource.isPlaying) proximitySource.Stop();
        }
    }
}