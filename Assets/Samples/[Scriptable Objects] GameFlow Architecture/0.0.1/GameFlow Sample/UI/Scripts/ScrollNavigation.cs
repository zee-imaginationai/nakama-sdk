using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScrollNavigation : MonoBehaviour
{
    [SerializeField] private RectTransform scroll;
    [SerializeField] private RectTransform[] screens;

    private int currentIndex;
    private Snap snapController;

    private void Start()
    {
        currentIndex = 0;
        snapController = new Snap(screens, scroll, 0);
    }

    private void Update()
    {
        snapController.Snaps();

    }

    public void OnDragStart()
    {
        snapController.OnDragStart();
    }

    public void OnDragEnd()
    {
        snapController.OnDragEnd();
        currentIndex = snapController.CurrentRect;
    }

    public void NextScreen()
    {
        currentIndex++;
        snapController.SetSnapIndex(currentIndex);
       
    }


}
