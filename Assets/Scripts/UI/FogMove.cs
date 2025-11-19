using UnityEngine;
using UnityEngine.UI;

public class FogMove : MonoBehaviour
{
    public float speedX = 0.02f;
    public float speedY = 0.01f;

    private RawImage fog;
    private Vector2 offset;

    void Start()
    {
        fog = GetComponent<RawImage>();
    }

    void Update()
    {
        offset.x += speedX * Time.deltaTime;
        offset.y += speedY * Time.deltaTime;
        fog.uvRect = new Rect(offset, fog.uvRect.size);
    }
}
