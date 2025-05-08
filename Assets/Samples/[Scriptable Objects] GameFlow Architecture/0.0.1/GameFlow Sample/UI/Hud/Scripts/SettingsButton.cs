using DG.Tweening;
using ProjectCore.Events;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectCore.UI
{
    public class SettingsButton : MonoBehaviour
    {
        [Header("Event Raiser")]
        [SerializeField] private GameEventWithBool SettingsEvent;
        
        [Header("Button Reference")]
        [SerializeField] private Button Button;
        
        [Header("Advanced Options Check")]
        [SerializeField] private bool HideAdvancedOptions;


       /* [SerializeField] private UISounds UISounds;*/

        #region Mono Functions
        
        private void OnEnable()
        {
            Button.onClick.AddListener(OnButtonPressed);
        }

        private void OnDisable()
        {
            Button.onClick.RemoveListener(OnButtonPressed);
        }

        #endregion

        #region Button CallBack

        private void OnButtonPressed()
        {
           /* UISounds.PlaySound();*/
            Button.interactable = false;
            DOVirtual.DelayedCall(0.1f, () =>
            {
                SettingsEvent.Raise(HideAdvancedOptions);
            });
            
            DOVirtual.DelayedCall(0.15f, () =>
            {
                Button.interactable = true;
            });
        }
        

        #endregion
        
    }
}
