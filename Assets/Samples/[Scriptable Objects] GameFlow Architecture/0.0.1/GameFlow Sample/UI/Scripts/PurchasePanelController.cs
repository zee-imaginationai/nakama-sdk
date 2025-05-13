using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using ProjectCore.Events;

namespace ProjectCore.UI
{
    public class PurchasePanelController : MonoBehaviour
    {
        [SerializeField] private Image PanelImage;
        [SerializeField] private GameEventWithBool StoreIAPViewChanged;
        
        private void Awake()
        {
            StoreIAPViewChanged.Handler += OnPurchasedItem;
        }
        
        private void OnDisable()
        {
            StoreIAPViewChanged.Handler -= OnPurchasedItem;
        }

        private void OnPurchasedItem(bool status)
        {
            if (status)
            {
                PanelImage.gameObject.SetActive(true);
                PanelImage.DOFade(0.9f, 0.4f);
            }
            else
            {
                PanelImage.DOFade(0, 0.4f).OnComplete(() => { PanelImage.gameObject.SetActive(false); });
            }

        }

    }
}