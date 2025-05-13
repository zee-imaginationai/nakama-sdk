using System;
using DG.Tweening;
using UnityEngine;

public class UIElementBounce : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;

    private Sequence _sequence;

    private void OnEnable()
    {
        StartBouncingTween();
    }

    private void OnDisable()
    {
        _sequence?.Kill();
    }

    void StartBouncingTween()
    {
        _sequence?.Kill();
        
        _sequence = DOTween.Sequence().Append(_rectTransform.DOScale(_rectTransform.localScale.x + .1f, 1f).SetEase(Ease.OutSine))
            .Append(_rectTransform.DOScale(_rectTransform.localScale.x, 1f).SetEase(Ease.InSine)).SetLoops(-1);
        _sequence.Play();
    }
}
