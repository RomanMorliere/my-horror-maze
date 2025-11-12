using UnityEngine;

public class SonarPulse : MonoBehaviour
{
    [Header("Sonar Settings")]
    public Material sonarMaterial;     // The material using your Shader Graph
    public float waveSpeed = 10f;      // How fast the wave expands
    public float waveWidth = 1f;       // How thick the wave is
    public KeyCode activationKey = KeyCode.Space;

    private float currentDistance = 0f;
    private bool isActive = false;

    void Update()
    {
        // Trigger the wave
        if (Input.GetKeyDown(activationKey))
        {
            isActive = true;
            currentDistance = 0f;
        }

        if (isActive)
        {
            currentDistance += Time.deltaTime * waveSpeed;

            // Send data to the shader
            sonarMaterial.SetVector("_PlayerPosition", transform.position);
            sonarMaterial.SetFloat("_WaveDistance", currentDistance);
            sonarMaterial.SetFloat("_WaveWidth", waveWidth);

            // Stop the wave after it expands far enough
            if (currentDistance > 50f)
                isActive = false;
        }
    }
}