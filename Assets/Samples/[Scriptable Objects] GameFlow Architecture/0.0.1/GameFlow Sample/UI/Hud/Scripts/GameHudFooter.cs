using UnityEngine;
using ProjectCore.Variables;
using Sirenix.OdinInspector;

namespace ProjectCore.Hud
{
    public class GameHudFooter : GameHudBar
    {

        [SerializeField] private bool HasCornerButtons;
        [SerializeField] private bool HasBanner;
        public GameObject HorizontalBarOverlay;
        
        [ShowIf("HasCornerButtons")]
        [SerializeField] private RectTransform InnerContainer;
        [ShowIf("HasCornerButtons")]
        [SerializeField] private RectTransform CornerButtons;
        [ShowIf("HasCornerButtons")]
        [SerializeField] private Bool CornerButtonsStatus;

        
        [ShowIf("HasBanner")]
        [SerializeField] private Bool BannerStatus;
        [ShowIf("HasBanner")]
        [SerializeField] private Float BannerHeight;


        public override void Show()
        {
            AnimateY(GameHudConfig.FooterOffScreenPosition, 0);
           
        }

        public override void Hide()
        {
            AnimateY(0, GameHudConfig.FooterOffScreenPosition);

        }
       
        public void OnBannerShown()
        {
            UpdateFooterStatus(true);
        }

        public void OnBannerHidden()
        {
            UpdateFooterStatus(false);
        }

        public void UpdateFooterStatus(bool bannerStatus)
        {
            Vector2 position = InnerContainer.anchoredPosition;
            if (bannerStatus)
            {
                position.y = BannerHeight;
            }
            else
            {
                position.y = 0;
            }

            InnerContainer.anchoredPosition = position;
        }

        private void OnEnable()
        {
            if (HasCornerButtons)
            {
                CornerButtons.gameObject.SetActive(CornerButtonsStatus.GetValue());
            }

            if (HasBanner)
            {
                UpdateFooterStatus(BannerStatus);
            }
        }
    }
}