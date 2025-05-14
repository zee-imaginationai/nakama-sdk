using System.Threading.Tasks;
#if NAKAMA_ENABLED
using ProjectCore.Integrations.Internal;
#endif
using UnityEngine;
using UnityEngine.UI;

public class SyncConflictPanel : MonoBehaviour
{
    [SerializeField] private Button ChooseLocalProgressButton;
    [SerializeField] private Button ChooseCloudProgressButton;
    [SerializeField] private GameObject PanelObject;

    [SerializeField] private MainMenuState MainMenuState;
    
#if NAKAMA_ENABLED
    private StorageType _storageType = StorageType.None;
    private bool _canSync = true;

    private void RegisterEvents()
    {
        ChooseLocalProgressButton.onClick.AddListener(OnChooseLocalProgressButtonPressed);
        ChooseCloudProgressButton.onClick.AddListener(OnChooseCloudProgressButtonPressed);
    }
    
    private void UnRegisterEvents()
    {
        ChooseLocalProgressButton.onClick.RemoveListener(OnChooseLocalProgressButtonPressed);
        ChooseCloudProgressButton.onClick.RemoveListener(OnChooseCloudProgressButtonPressed);
    }
    
    private void OnChooseLocalProgressButtonPressed()
    {
        _storageType = StorageType.Local;
        _canSync = false;
        MainMenuState.DisconnectFacebook();
    }

    private void OnChooseCloudProgressButtonPressed()
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
    
    private void SetSyncConflictPanelState(bool state)
    {
        PanelObject.SetActive(state);
    }
    
    public async Task<(StorageType, bool)> Show()
    {
        RegisterEvents();
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