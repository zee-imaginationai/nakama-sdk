using ProjectCore.Events;

#if FB
using ProjectCore.Integrations.FacebookService;
#endif

using ProjectCore.Variables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthPanelView : MonoBehaviour
{
    [SerializeField] private Button CloseButton;

    [SerializeField] private GameObject AuthPanelObject;

    [SerializeField] private Button FbLoginButton;
    
    [SerializeField] private TextMeshProUGUI FbLoginText;
    
#if FB
    [SerializeField] private FacebookService FacebookService;
#endif
    
    [SerializeField] private DBBool IsFbLoggedIn;

    [SerializeField] private GameEventWithBool FacebookConnectEvent;

    private void OnEnable()
    {
        CloseButton.onClick.AddListener(OnCloseButton);
        FbLoginButton.onClick.AddListener(OnFbLoginButton);

        FacebookConnectEvent.Handler += OnFacebookConnectEvent;
    }

    private void OnDisable()
    {
        CloseButton.onClick.RemoveListener(OnCloseButton);
        FbLoginButton.onClick.RemoveListener(OnFbLoginButton);
        
        FacebookConnectEvent.Handler -= OnFacebookConnectEvent;
    }

    private void UpdatePanel()
    {
        OnFacebookConnectEvent(IsFbLoggedIn);
    }

    public void Show()
    {
        UpdatePanel();
        SetAuthPanelState(true);
    }

    public void Hide() => SetAuthPanelState(false);

    private void OnFacebookConnectEvent(bool isConnected)
    {
        FbLoginButton.image.color = isConnected ? Color.green : Color.red;
        FbLoginText.text = isConnected ? "Disconnect" : "Connect";
    }

    private void OnFbLoginButton()
    {
#if FB
        if (IsFbLoggedIn)
            FacebookService.LogoutFacebook();
        else
            FacebookService.LoginFacebook();
#else
        Debug.LogError("FB is not connected");
#endif
    }
    
    private void OnCloseButton() => SetAuthPanelState(false);

    private void SetAuthPanelState(bool state) => AuthPanelObject.SetActive(state);
}