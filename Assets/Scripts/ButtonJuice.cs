using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonJuice : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float _pressedScale = 0.94f;
    [SerializeField] private float _pressDuration = 0.06f;
    [SerializeField] private float _releaseDuration = 0.12f;
    // what gets squished; leave empty for this transform. Cell points it at a child
    // so the juice doesn't fight Cell's own Appear/Punch tweens on the root.
    [SerializeField] private Transform _target;

    private Transform Target => _target ? _target : transform;

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
        Target.DOKill();
        Target.localScale = Vector3.one;
    }

    private void ScaleTo(float scale, float duration, Ease ease)
    {
        Target.DOKill();
        Target.DOScale(scale, duration)
                 .SetEase(ease)
                 .SetUpdate(true)
                 .SetLink(gameObject);
    }
}
