using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonJuice : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float _pressedScale = 0.94f;
    [SerializeField] private float _pressDuration = 0.06f;
    [SerializeField] private float _releaseDuration = 0.12f;

    public void OnPointerDown(PointerEventData eventData)
    {
        ScaleTo(_pressedScale, _pressDuration, Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ScaleTo(1f, _releaseDuration, Ease.OutBack);
    }

    private void OnDisable()
    {
        transform.DOKill();
        transform.localScale = Vector3.one;
    }

    private void ScaleTo(float scale, float duration, Ease ease)
    {
        transform.DOKill();
        transform.DOScale(scale, duration)
                 .SetEase(ease)
                 .SetUpdate(true)
                 .SetLink(gameObject);
    }
}
