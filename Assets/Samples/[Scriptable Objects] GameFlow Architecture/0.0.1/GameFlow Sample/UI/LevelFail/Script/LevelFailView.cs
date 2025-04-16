using ProjectCore.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelFailView : UiPanelInAndOut
{
    [SerializeField] private Button NextButton;

    [SerializeField] private GameEvent GotoMainMenu;


    private void OnEnable()
    {
        NextButton.onClick.AddListener(OnNextButtonPressed);
    }

    private void OnDisable()
    {
        NextButton.onClick.RemoveListener(OnNextButtonPressed);
    }

    private void OnNextButtonPressed()
    {
        GotoMainMenu.Invoke();
    }
}
