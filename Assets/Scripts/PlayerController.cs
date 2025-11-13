using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float fixedY = 0.5f;

    private Rigidbody rb;
    private Vector3 moveInput;
    private Vector3 moveVelocity;

    public bool IsBoosted { get; private set; } = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ |
                         RigidbodyConstraints.FreezePositionY;
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(moveX, 0f, moveZ).normalized;
        moveVelocity = moveInput * moveSpeed;

        // Fix the rotation
        transform.rotation = Quaternion.identity;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(moveVelocity.x, 0f, moveVelocity.z);
        rb.position = new Vector3(rb.position.x, fixedY, rb.position.z);
    }

    public IEnumerator ApplySpeedBoost(float boostAmount, float duration)
    {
        if (IsBoosted)
            yield break;

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
}