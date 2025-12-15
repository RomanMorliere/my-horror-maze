using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")] 
    public float moveSpeed = 5f;
    public float fixedY = 0.5f;

    [Header("Inventory (Charges)")]
    public int wallBreakerCharges = 0;
    public int shieldCharges = 0;
    public int revealCharges = 0;
    public int speedCharges = 0;

[Header("Audio (Drag your sounds here)")]
public AudioSource hammerSFX;
public AudioSource speedSFX;
public AudioSource shieldSFX;
public AudioSource revealSFX;

    [Header("Boost Durations")]
    public float shieldDuration = 5f;
    public float revealDuration = 3f;

    private Rigidbody rb;
    private Vector3 moveInput; // Added back
    private Vector3 moveVelocity; // Added back

    public bool IsBoosted { get; private set; } = false;
    public bool IsInvincible { get; private set; } = false;
    private Vector3 lastMoveDirection = Vector3.forward; // Default to facing forward

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        
        // Using your original working constraints
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ |
                         RigidbodyConstraints.FreezePositionY;
    }

    void Update()
    {
        // 1. WORKING MOVEMENT LOGIC (Restored from your old code)
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(moveX, 0f, moveZ).normalized;
        moveVelocity = moveInput * moveSpeed;

        transform.rotation = Quaternion.identity;

        if (moveInput != Vector3.zero)
    {
        lastMoveDirection = moveInput;
    }

        // 2. USE BOOSTS WITH KEYS
// SPACE -> Break Wall
    if (Input.GetKeyDown(KeyCode.Space) && wallBreakerCharges > 0)
    {
        if (hammerSFX != null) hammerSFX.Play(); // Plays Hammer Sound
        TryBreakWall();
    }

    // R -> Use Speed Boost
    if (Input.GetKeyDown(KeyCode.R) && speedCharges > 0 && !IsBoosted)
    {
        if (speedSFX != null) speedSFX.Play(); // Plays Speed Sound
        speedCharges--;
        StartCoroutine(ApplySpeedBoost(2f, 5f));
    }

    // Q -> Use Shield
    if (Input.GetKeyDown(KeyCode.Q) && shieldCharges > 0 && !IsInvincible)
    {
        if (shieldSFX != null) shieldSFX.Play(); // Plays Shield Sound
        shieldCharges--;
        StartCoroutine(ApplySheild(shieldDuration));
    }

    // E -> Use Reveal
    if (Input.GetKeyDown(KeyCode.E) && revealCharges > 0)
    {
        if (revealSFX != null) revealSFX.Play(); // Plays Reveal Sound
        revealCharges--;
        MazeGenerator gen = Object.FindFirstObjectByType<MazeGenerator>();
        if (gen != null) StartCoroutine(gen.RevealWalls(revealDuration));
    }
    }

    void FixedUpdate() 
    { 
        // Using your original working physics movement
        rb.linearVelocity = new Vector3(moveVelocity.x, 0f, moveVelocity.z);
        rb.position = new Vector3(rb.position.x, fixedY, rb.position.z);
    }

private void TryBreakWall()
{
    // 1. Use the "Memory" direction we saved earlier
    Vector3 lookDirection = lastMoveDirection;

    // 2. Shoot the Raycast (raised slightly off the floor)
    Vector3 rayStart = transform.position + Vector3.up * 0.5f; 
    RaycastHit hit;
    float breakDistance = 1.8f; 

    // Visual Debug: This draws a line in the Scene View for 2 seconds
    Debug.DrawRay(rayStart, lookDirection * breakDistance, Color.green, 2f);

    if (Physics.Raycast(rayStart, lookDirection, out hit, breakDistance))
    {
        if (hit.collider.name.Contains("Wall"))
        {
            MazeNode node = hit.collider.GetComponentInParent<MazeNode>();
            if (node != null)
            {
                node.DestroyClosestWall(transform.position);
                wallBreakerCharges--;
                Debug.Log($"🔨 Smashed wall using saved direction: {lookDirection}");
            }
        }
    }
    else 
    {
        Debug.Log("No wall detected in the last direction you moved.");
    }
}

    // Methods to add charges (called by pickups)
    public void AddWallBreakerCharges(int amount) => wallBreakerCharges += amount;
    public void AddShieldCharge() => shieldCharges++;
    public void AddRevealCharge() => revealCharges++;
    public void AddSpeedCharge() => speedCharges++;

    // --- COROUTINES (Visual Effects) ---

    public IEnumerator ApplySpeedBoost(float boostAmount, float duration)
    {
        if (IsBoosted) yield break;
        IsBoosted = true;
        float originalSpeed = moveSpeed;
        moveSpeed *= boostAmount;
        Renderer rend = GetComponent<Renderer>();
        Color originalColor = rend.material.color;
        rend.material.color = Color.yellow;
        yield return new WaitForSeconds(duration);
        moveSpeed = originalSpeed;
        IsBoosted = false;
        rend.material.color = originalColor;
    }

    public IEnumerator ApplySheild(float duration)
    {
        IsInvincible = true;
        Renderer rend = GetComponent<Renderer>();
        Color originalColor = rend.material.color;
        rend.material.color = Color.blue;
        yield return new WaitForSeconds(duration);
        IsInvincible = false;
        rend.material.color = originalColor;
    }
}