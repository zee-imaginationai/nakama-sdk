using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : UiPanelInAndOut
{
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button EmailAuthButton;

    [SerializeField] private CloudSyncPanel CloudSyncPanel;
    
    [SerializeField] private MainMenuState MainMenuState;

    private void OnEnable()
    {
        PlayButton.onClick.AddListener(OpenViewButtonPressed);
        EmailAuthButton.onClick.AddListener(OnEmailAuthButton);
        CloudSyncPanel.Hide();
    }

    private void OnDisable()
    {
        PlayButton.onClick.RemoveListener(OpenViewButtonPressed);
        EmailAuthButton.onClick.RemoveListener(OnEmailAuthButton);
    }
    
    private void OpenViewButtonPressed()
    {
        MainMenuState.GotoGame();
    }

    public void ShowErrorText(string errorText)
    {
        CloudSyncPanel.ShowErrorText(errorText);
    }

    private void OnEmailAuthButton()
    {
        CloudSyncPanel.Show();
    }
}