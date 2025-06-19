using System.Threading.Tasks;
using ProjectCore.Variables;
using TMPro;

#if FB
using ProjectCore.Integrations.FacebookService;
#endif

#if NAKAMA_ENABLED
using ProjectCore.Integrations.Internal;
#endif

using UnityEngine;
using UnityEngine.UI;

public class ConflictResolutionPanelView : MonoBehaviour
{
    [SerializeField] private Button DeclineButton;
    [SerializeField] private Button AcceptButton;
    [SerializeField] private GameObject PanelObject;
    
    [SerializeField] private TextMeshProUGUI WarningText;
    
    [SerializeField] private DBBool IsFbLoggedIn;

#if FB
    [SerializeField] private FacebookService FacebookService;
#endif
    
#if NAKAMA_ENABLED
    private StorageType _storageType = StorageType.None;
    private bool _canSync = true;

    private const string FB_LOGIN_MESSAGE = "Do you wish to load game progress from this Facebook account?" +
                                            " Doing so will discard progress from the existing device.";
    
    private const string FB_LOGOUT_MESSAGE = "Do you wish to unbind your game account from Facebook Sign-In?" +
                                             " You may stand to lose game progress while doing so.";

    private void RegisterEvents()
    {
        DeclineButton.onClick.AddListener(OnDeclineButtonPressed);
        AcceptButton.onClick.AddListener(OnAcceptButtonPressed);
    }
    
    private void UnRegisterEvents()
    {
        DeclineButton.onClick.RemoveListener(OnDeclineButtonPressed);
        AcceptButton.onClick.RemoveListener(OnAcceptButtonPressed);
    }
    
    private void OnDeclineButtonPressed()
    {
        _storageType = StorageType.Local;
        _canSync = false;
        
#if FB
        if(IsFbLoggedIn)
            FacebookService.LogoutFacebook();
        else
            FacebookService.LoginFacebook();
#endif
    }

    private void OnAcceptButtonPressed()
    {
        _storageType = StorageType.Cloud;
        _canSync = true;
    }

    private async Task WaitUntilStorageChanged()
    {
        while (_storageType == StorageType.None)
        {
            await Task.Yield();
        }
    }

    private void UpdateWarningText()
    {
        WarningText.text = IsFbLoggedIn ? FB_LOGOUT_MESSAGE : FB_LOGIN_MESSAGE;
    }
    
    private void SetSyncConflictPanelState(bool state)
    {
        PanelObject.SetActive(state);
    }
    
    public async Task<(StorageType, bool)> Show()
    {
        RegisterEvents();
        UpdateWarningText();
        SetSyncConflictPanelState(true);
        _storageType = StorageType.None;

        await WaitUntilStorageChanged();
        
        return (_storageType, _canSync);
    }
    
    public void Hide()
    {
        SetSyncConflictPanelState(false);
        UnRegisterEvents();
    }
#endif
}