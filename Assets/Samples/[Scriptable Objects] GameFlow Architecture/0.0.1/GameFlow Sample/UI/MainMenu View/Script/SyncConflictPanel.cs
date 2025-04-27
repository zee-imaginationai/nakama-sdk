using System.Threading.Tasks;
using ProjectCore.CloudService.Nakama.Internal;
using UnityEngine;
using UnityEngine.UI;

public class SyncConflictPanel : MonoBehaviour
{
    [SerializeField] private Button ChooseLocalProgressButton;
    [SerializeField] private Button ChooseCloudProgressButton;
    [SerializeField] private GameObject PanelObject;
    
    private StorageType _storageType = StorageType.None;

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
    
    private void OnChooseLocalProgressButtonPressed() => _storageType = StorageType.Local;
    private void OnChooseCloudProgressButtonPressed() => _storageType = StorageType.Cloud;

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
    
    public async Task<StorageType> Show()
    {
        RegisterEvents();
        SetSyncConflictPanelState(true);
        _storageType = StorageType.None;

        await WaitUntilStorageChanged();
        
        return _storageType;
    }
    
    public void Hide()
    {
        SetSyncConflictPanelState(false);
        UnRegisterEvents();
    }
}