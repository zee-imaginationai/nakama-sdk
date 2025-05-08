using System.Collections;
using System.Collections.Generic;
using ProjectCore.Events;
using UnityEngine;
using UnityEngine.UI;

public class ScreenOverlay : MonoBehaviour
{
    [SerializeField] private GameObject Overlay;
    [SerializeField] private GameEventWithIntStringBool OverlayShowHideEvent;
    private Canvas _overlayCanvas;
    //public SortingLayer SortingLayer;
    private void Start()
    {
        _overlayCanvas = Overlay.GetComponent<Canvas>();
    }
    private void OnEnable()
    {
        OverlayShowHideEvent.Handler += OverlayShowHide;
    }

    private void OnDisable()
    {
        OverlayShowHideEvent.Handler -= OverlayShowHide;
    }

    private void OverlayShowHide(int sortingOder,string sortingLayer,bool show)
    {
        Overlay.SetActive(show);
        _overlayCanvas.sortingOrder = sortingOder;
        _overlayCanvas.sortingLayerName = sortingLayer;
        _overlayCanvas.overrideSorting = true;

    }
}
