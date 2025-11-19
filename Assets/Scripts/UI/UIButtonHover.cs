using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    public float scaleAmount = 1.1f;
    public float animationSpeed = 8f;

    private bool isHovering = false;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        Vector3 targetScale = originalScale * (isHovering ? scaleAmount : 1f);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
}
