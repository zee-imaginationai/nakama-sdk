using System;
using System.Collections;
using System.Collections.Generic;
using CustomUtilities;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace ProjectCore.UI
{
    public class RateUsView : MonoBehaviour
    {
        #region Serialized Variables

        [SerializeField] private Button CloseButton;
        [SerializeField] private Button RateUsButton;
        [SerializeField] private UIViewState RateUsState;

        /*[Header("Android InApp Review")] 
        [SerializeField] private AndroidInAppReview AndroidInAppReview;*/

        #endregion

        #region Mono Methods

        private void OnEnable()
        {
            RegisterListeners();
        }

        private void OnDisable()
        {
            UnregisterListeners();
        }

        #endregion

        #region Buttons Register & Unregister

        private void RegisterListeners()
        {
            CloseButton.onClick.AddListener(CloseButtonPressed);
            RateUsButton.onClick.AddListener(RateUsButtonPressed);
        }

        private void UnregisterListeners()
        {
            CloseButton.onClick.RemoveListener(CloseButtonPressed);
            RateUsButton.onClick.RemoveListener(RateUsButtonPressed);
        }

        #endregion

        #region Button Callbacks

        void CloseButtonPressed()
        {
            RateUsState.CloseView();
        }
        
        void RateUsButtonPressed()
        {
        #if UNITY_IOS
            Device.RequestStoreReview();
#else 
            // AndroidInAppReview.TriggerInAppRatingFlow(this);
#endif
            CloseButtonPressed();
        }

#endregion
    }
}