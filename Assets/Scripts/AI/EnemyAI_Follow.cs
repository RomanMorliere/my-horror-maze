using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class EnemyAI_Follow : MonoBehaviour
{
    public float speed = 3f;
    public float turnSpeed = 8f;
    public float stoppingDistance = 0.3f;

    private Rigidbody rb;
    private List<MazeNode> path;
    private int currentIndex = 0;
    private Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // no tipping over
    }

    public void SetTarget(Transform p)
    {
        player = p;
    }

    public void SetPath(List<MazeNode> newPath)
    {
        path = newPath;
        currentIndex = 0;
    }

    void FixedUpdate()
    {
        if (path == null || path.Count == 0) {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        if (currentIndex >= path.Count) {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        // Where we want to go next
        Vector3 targetPos = path[currentIndex].transform.position;
        targetPos.y = transform.position.y;

        // Direction
        Vector3 dir = (targetPos - transform.position);
        dir.y = 0;
        float distance = dir.magnitude;

        if (distance < stoppingDistance)
        {
            currentIndex++; // go to next tile in path
            return;
        }

        Vector3 moveDir = dir.normalized;

        // Move
        rb.linearVelocity = moveDir * speed;

        // Rotate smoothly
        if (moveDir != Vector3.zero)
        {
            Quaternion goalRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, goalRot, turnSpeed * Time.deltaTime);
        }
    }
}
