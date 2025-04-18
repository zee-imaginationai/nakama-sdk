using ProjectCore.Variables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CloudSyncPanel : MonoBehaviour
{
    [SerializeField] private Button CloseButton;

    [SerializeField] private GameObject AuthPanelObject;
    [SerializeField] private GameObject ErrorObject;
    
    [SerializeField] private Button RegisterButton;
    [SerializeField] private Button LoginButton;
    [SerializeField] private Button LogOutButton;
    
    [SerializeField] private TMP_InputField EmailInputField;
    [SerializeField] private TMP_InputField PasswordInputField;
    [SerializeField] private TextMeshProUGUI ErrorText;
    
    [SerializeField] private MainMenuState MainMenuState;
    
    [SerializeField] private Bool IsEmailLoggedIn;

    private string _email;
    private string _password;

    private void OnEnable()
    {
        RegisterButton.onClick.AddListener(OnRegisterButtonPressed);
        LoginButton.onClick.AddListener(OnLoginButtonPressed);
        LogOutButton.onClick.AddListener(OnLogOutButtonPressed);
        EmailInputField.onSubmit.AddListener(OnEmailSubmit);
        CloseButton.onClick.AddListener(OnCloseButton);
        PasswordInputField.onSubmit.AddListener(OnPasswordSubmit);
        SetErrorState(false);
    }

    private void OnDisable()
    {
        RegisterButton.onClick.RemoveListener(OnRegisterButtonPressed);
        LoginButton.onClick.RemoveListener(OnLoginButtonPressed);
        LogOutButton.onClick.RemoveListener(OnLogOutButtonPressed);
        EmailInputField.onSubmit.RemoveListener(OnEmailSubmit);
        CloseButton.onClick.RemoveListener(OnCloseButton);
        PasswordInputField.onSubmit.RemoveListener(OnPasswordSubmit);
        SetErrorState(false);
    }

    private void OnLogOutButtonPressed()
    {
        MainMenuState.LogoutNakama();
    }

    private void OnLoginButtonPressed()
    {
        
    }

    private void UpdatePanel()
    {
        LogOutButton.gameObject.SetActive(IsEmailLoggedIn);
        LoginButton.gameObject.SetActive(!IsEmailLoggedIn);
        RegisterButton.gameObject.SetActive(!IsEmailLoggedIn);
        EmailInputField.gameObject.SetActive(!IsEmailLoggedIn);
        PasswordInputField.gameObject.SetActive(!IsEmailLoggedIn);
    }

    public void Show()
    {
        UpdatePanel();
        SetAuthPanelState(true);
    }

    public void Hide() => SetAuthPanelState(false);
    
    private void OnCloseButton() => SetAuthPanelState(false);

    private void SetAuthPanelState(bool state) => AuthPanelObject.SetActive(state);
    
    private void SetErrorState(bool state) => ErrorObject.SetActive(state);

    private void OnEmailSubmit(string email) => _email = email;
    
    private void OnPasswordSubmit(string password) => _password = password;
    
    public void ShowErrorText(string errorText)
    {
        ErrorText.text = errorText;
        SetErrorState(true);
    }

    private void OnRegisterButtonPressed()
    {
        MainMenuState.EmailSyncNakama(_email, _password);
    }
}