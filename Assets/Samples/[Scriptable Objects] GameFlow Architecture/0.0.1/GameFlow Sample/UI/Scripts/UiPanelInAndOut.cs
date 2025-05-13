using System.Collections;
using UnityEngine;
using DG.Tweening;
using ExtensionMethods;
using Sirenix.OdinInspector;

public class UiPanelInAndOut : UIView
{
    public UIConfig UIPanelConfig;
    public RectTransform UIPanel;
    /*[Required] public UISounds UISounds;*/

    protected float _exitTime;

    private Tween _panelTranslationTween;
    
    private void Awake()
    {
        UIPanel.SetAnchoredPositionX(UIPanelConfig.GetOffScreenOffset());
    }

    public override IEnumerator Show(bool isResumed)
    {
        if (isResumed)
            UIPanel.SetAnchoredPositionX(UIPanelConfig.GetOffScreenOffset());
        
        UIPanelEaseIn();

        _exitTime = UIPanelConfig.EaseOutTime + 0.1f;
        yield return new WaitForSeconds(UIPanelConfig.EaseInTime);
    }

    public virtual void UIPanelEaseIn()
    {
        _panelTranslationTween?.Kill();
        _panelTranslationTween = UIPanel.DOAnchorPosX(0, UIPanelConfig.EaseInTime).SetEase(UIPanelConfig.EasingIn);
    }

    public override IEnumerator Hide(bool shouldDestroy)
    {
       /* UISounds.PlayCloseViewSound();*/
        UIPanelEaseOut(shouldDestroy);
        yield return new WaitForSeconds(_exitTime);
    }

    public virtual void UIPanelEaseOut(bool shouldDestroy)
    {
        _panelTranslationTween?.Kill();
        _panelTranslationTween = UIPanel.DOAnchorPosX(-UIPanelConfig.GetOffScreenOffset(), UIPanelConfig.EaseOutTime).SetEase(UIPanelConfig.EasingOut);

        if (shouldDestroy)
            Destroy(gameObject, _exitTime);
    }
}
