using UnityEngine;

using DG.Tweening;
using Sirenix.OdinInspector;

namespace ProjectCore.Hud
{
    public class GameHudBar : MonoBehaviour
    {
        [SerializeField] protected GameHudConfig GameHudConfig;
        [SerializeField] protected RectTransform RectTransform;

        private Tween _tween;

        [Button]
        public virtual void Show()
        {
            
        }

        [Button]
        public virtual void Hide()
        {
            
        }

        protected virtual void AnimateY(float startingY, float destinationY)
        {
            KillTween();

            RectTransform.anchoredPosition = new Vector2(RectTransform.anchoredPosition.x, startingY);
            _tween = RectTransform.DOAnchorPosY(destinationY, GameHudConfig.AnimationDuration).OnKill(() => {
                RectTransform.anchoredPosition = new Vector2(RectTransform.anchoredPosition.x, destinationY);
            }).OnComplete(()=> {
                _tween = null;
            });
        }
        
        protected virtual void AnimateX(float startingX, float destinationX)
        {
            KillTween();

            RectTransform.anchoredPosition = new Vector2(startingX, RectTransform.anchoredPosition.y);
            _tween = RectTransform.DOAnchorPosX(destinationX, GameHudConfig.AnimationDuration).OnKill(() => {
                RectTransform.anchoredPosition = new Vector2(destinationX, RectTransform.anchoredPosition.y);
            }).OnComplete(()=> {
                _tween = null;
            });
        }

        protected virtual void KillTween()
        {
            if (_tween != null)
            {
                _tween.Kill();
            }
            _tween = null;
        }
    }
}