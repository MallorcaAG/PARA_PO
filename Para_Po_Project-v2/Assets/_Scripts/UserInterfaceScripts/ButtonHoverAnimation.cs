using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonHoverAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale Settings")]
    public float hoverScale = 1.1f;
    public float duration = 0.2f;

    private Vector3 originalScale;
    private Tween scaleTween;
    private bool isHovered = false;
    private bool initialized = false;

    private void Init()
    {
        if (!initialized)
        {
            originalScale = transform.localScale;
            initialized = true;
        }
    }

    void OnEnable()
    {
        Init(); // Ensure we capture the scale
        transform.localScale = originalScale;
        isHovered = false;
        scaleTween?.Kill();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Init();
        if (isHovered) return;
        isHovered = true;

        scaleTween?.Kill();
        scaleTween = transform.DOScale(originalScale * hoverScale, duration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true); // Makes it work even when Time.timeScale == 0
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Init();
        if (!isHovered) return;
        isHovered = false;

        scaleTween?.Kill();
        scaleTween = transform.DOScale(originalScale, duration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);
    }
}
