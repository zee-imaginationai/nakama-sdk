using ProjectCore.Events;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : UiPanelInAndOut
{
    [SerializeField] private Button PlayButton;

    [SerializeField] private GameEvent ShowLevelCompleteView;

    [SerializeField] private MainMenuState MainMenuState;
    private void OnEnable()
    {
        PlayButton.onClick.AddListener(OpenViewButtonPressed);
    }

    private void OnDisable()
    {
        PlayButton.onClick.RemoveListener(OpenViewButtonPressed);
    }

    

    private void OpenViewButtonPressed()
    {
        /*  ShowLevelCompleteView.Invoke()*/
        MainMenuState.GotoGame();
    }
}
