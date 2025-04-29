using ProjectCore.Events;
using ProjectCore.StateMachine;
using System;
using System.Collections;
using System.Threading.Tasks;
using Nakama;
using ProjectCore.CloudService.Internal;
using ProjectCore.CloudService.Nakama;
using ProjectCore.CloudService.Nakama.Internal;
using UnityEngine;

[CreateAssetMenu(fileName = "MainMenuState", menuName = "ProjectCore/State Machine/States/MainMenu State")]
public class MainMenuState : State
{
    [SerializeField] private string PrefabName;

    [SerializeField] private GameEvent GotoGameEvent;
    [NonSerialized] private MainMenuView _mainMenuView;
    [SerializeField] private NakamaSystem NakamaSystem;
    [SerializeField] private NakamaUserProgressService UserProgressService;
    [SerializeField] private FacebookService FacebookService;
    public override IEnumerator Execute()
    {
        yield return base.Execute();
        _mainMenuView = Instantiate(Resources.Load<MainMenuView>(PrefabName));

        yield return _mainMenuView.Show(true);
    }

    public override IEnumerator Exit()
    {
        yield return _mainMenuView.Hide(true);
        _mainMenuView = null;
        yield return base.Exit();
    }
    
    public async void ConnectFacebook()
    {
        await FacebookService.LoginFacebook();
        // await NakamaSystem.SigninWithFacebook();
    }

    public void DisconnectFacebook()
    {
        FacebookService.LogoutFacebook();
    }

    public async Task<StorageType> ResolveConflict()
    {
        var storageType = await _mainMenuView.ShowSyncConflictPanel();
        return storageType;
    }

    public void UpdateButton() => _mainMenuView?.UpdateButton();

    private void OnConnectionWithEmail(bool success, ApiResponseException exception)
    {
        if(success)
            _mainMenuView.ShowErrorText("Sign In Success");
        else
        {
            _mainMenuView.ShowErrorText(exception.Message);
        }
    }

    public async void LogoutNakama()
    {
    }

    private void OnLogout(bool success, ApiResponseException exception)
    {
        if(success)
            _mainMenuView.ShowErrorText("Log Out Success");
        else
        {
            _mainMenuView.ShowErrorText(exception.Message);
        }
    }

    public void GotoGame()
    {
        GotoGameEvent.Invoke();
    }
}
