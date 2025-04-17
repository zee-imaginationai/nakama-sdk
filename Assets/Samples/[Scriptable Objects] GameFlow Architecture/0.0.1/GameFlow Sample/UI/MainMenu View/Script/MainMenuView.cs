using ProjectCore.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : UiPanelInAndOut
{
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button EmailAuthButton;

    [SerializeField] private GameObject AuthPanelObject;
    [SerializeField] private Button LoginButton;
    [SerializeField] private TMP_InputField EmailInputField;
    [SerializeField] private TMP_InputField PasswordInputField;

    [SerializeField] private GameEvent ShowLevelCompleteView;

    [SerializeField] private MainMenuState MainMenuState;

    private string _email;
    private string _password;
    
    private void OnEnable()
    {
        PlayButton.onClick.AddListener(OpenViewButtonPressed);
        EmailAuthButton.onClick.AddListener(OnEmailAuthButton);
        LoginButton.onClick.AddListener(OnLoginButtonPressed);
        EmailInputField.onSubmit.AddListener(OnEmailSubmit);
        PasswordInputField.onSubmit.AddListener(OnPasswordSubmit);
    }

    private void OnDisable()
    {
        PlayButton.onClick.RemoveListener(OpenViewButtonPressed);
        EmailAuthButton.onClick.RemoveListener(OnEmailAuthButton);
        LoginButton.onClick.RemoveListener(OnLoginButtonPressed);
        EmailInputField.onSubmit.RemoveListener(OnEmailSubmit);
        PasswordInputField.onSubmit.RemoveListener(OnPasswordSubmit);
    }

    private void OnEmailSubmit(string email) => _email = email;
    
    private void OnPasswordSubmit(string password) => _password = password;

    private void OnEmailAuthButton()
    {
        AuthPanelObject.SetActive(true);
    }

    private void OnLoginButtonPressed()
    {
        AuthPanelObject.SetActive(false);
        MainMenuState.EmailSyncNakama(_email, _password);
    }

    private void OpenViewButtonPressed()
    {
        /*  ShowLevelCompleteView.Invoke()*/
        MainMenuState.GotoGame();
    }
}
