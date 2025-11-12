using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 15f, 0);
    public float smoothSpeed = 5f;

    private void LateUpdate()
    {
        if (!target) return;

        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}