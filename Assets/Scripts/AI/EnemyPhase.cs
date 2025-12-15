using UnityEngine;
using System.Collections;

public class EnemyPhase : MonoBehaviour
{
    [Header("Phase Settings")]
    public float invisibleDuration = 5f; 
    public float visibleDuration = 3f;   

    private MeshRenderer enemyRenderer;

    void Awake()
    {
        // We find the Renderer once at the very start
        enemyRenderer = GetComponentInChildren<MeshRenderer>();
    }

    void Start()
    {
        // Start the infinite loop
        StartCoroutine(PhaseCycle());
    }

    IEnumerator PhaseCycle()
    {
        while (true)
        {
            // --- 1. VISIBLE STATE ---
            SetVisibility(true);
            yield return new WaitForSeconds(visibleDuration);

            // --- 2. INVISIBLE STATE ---
            SetVisibility(false);
            yield return new WaitForSeconds(invisibleDuration);
        }
    }

    void SetVisibility(bool isVisible)
    {
        // We ONLY turn the drawing (Renderer) off/on
        // The AI script is NOT touched, so he keeps walking
        if (enemyRenderer != null) 
        {
            enemyRenderer.enabled = isVisible;
        }

        Debug.Log(isVisible ? "👻 Enemy Visible & Stalking" : "🌑 Enemy Invisible & Stalking");
    }
}