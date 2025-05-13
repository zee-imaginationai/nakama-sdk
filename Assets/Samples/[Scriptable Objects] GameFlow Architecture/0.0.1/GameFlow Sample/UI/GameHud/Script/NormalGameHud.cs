using ProjectCore.Events;
using ProjectCore.Hud;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NormalGameHud : GameHud
{
    [SerializeField] private Button LevelCompleteButton;
    [SerializeField] private GameEvent ShowLevelComplete;


    [SerializeField] private Button LevelFailButton;
    [SerializeField] private GameEvent ShowLevelFailView;

    private void OnEnable()
    {
        LevelCompleteButton.onClick.AddListener(OnLevelCompleteButtonPressed);
        LevelFailButton.onClick.AddListener(OnLevelFailButtonPressed);
    }

    private void OnDisable()
    {
        LevelCompleteButton.onClick.RemoveListener(OnLevelCompleteButtonPressed);
        LevelFailButton.onClick.RemoveListener(OnLevelFailButtonPressed);
    }

    private void OnLevelCompleteButtonPressed()
    {
        ShowLevelComplete.Invoke();
    }

    private void OnLevelFailButtonPressed()
    {
        ShowLevelFailView.Invoke();
    }
}
