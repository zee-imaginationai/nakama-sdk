using DG.Tweening;
using Sirenix.OdinInspector;
using ProjectCore.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class ButtonClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [FormerlySerializedAs("_rectTransform")] [SerializeField] private RectTransform RectTransform;
    
    [SerializeField] private Vector3 Scale;
    [SerializeField] private float ScaleInTime;
    [SerializeField] private float ScaleOutTime;
    [SerializeField] private float ScaleBackTime;

    private Vector3 _originalScale;
    private Sequence _sequence;
    private bool hasPlayed = false;

    private void OnEnable()
    {
        _originalScale = RectTransform.localScale;
    }

    private void OnDisable()
    {
        _sequence?.Kill();
    }

    public void OnButtonPressed()
    {
    }

    private void FeedbackTween()
    {
        _sequence?.Kill();

        RectTransform.localScale = _originalScale;

        _sequence = DOTween.Sequence()
            .Append(RectTransform.DOScale(_originalScale - Scale, ScaleInTime).SetEase(Ease.InBack))
            .Append(RectTransform.DOScale(_originalScale + Scale, ScaleOutTime).SetEase(Ease.InOutBack))
            .Append(RectTransform.DOScale(_originalScale, ScaleBackTime).SetEase(Ease.OutBack));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!hasPlayed)
        {
            hasPlayed = true;
            FeedbackTween();
            DOVirtual.DelayedCall(0.5f, () =>
            {
                hasPlayed = false;
            });
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        hasPlayed = false;
    }
}