using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float fixedY = 0.5f;

    private Rigidbody rb;
    private Vector3 moveInput;
    private Vector3 moveVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ |
                         RigidbodyConstraints.FreezePositionY;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(moveX, 0f, moveZ).normalized;
        moveVelocity = moveInput * moveSpeed;
    }

    void FixedUpdate()
    {
        Vector3 newPosition = rb.position + moveVelocity * Time.fixedDeltaTime;
        newPosition.y = fixedY;

        rb.MovePosition(newPosition);
    }
}