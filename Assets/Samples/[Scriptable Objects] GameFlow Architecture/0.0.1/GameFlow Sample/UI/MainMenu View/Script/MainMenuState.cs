using ProjectCore.Events;
using ProjectCore.StateMachine;
using System;
using System.Collections;
using System.Threading.Tasks;
using ProjectCore.Integrations.FacebookService;
#if NAKAMA_ENABLED
using Nakama;
using ProjectCore.Integrations.Internal;
using ProjectCore.Integrations.NakamaServer;
using ProjectCore.Integrations.NakamaServer.Internal;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "MainMenuState", menuName = "ProjectCore/State Machine/States/MainMenu State")]
public class MainMenuState : State
{
    [SerializeField] private string PrefabName;

    [SerializeField] private GameEvent GotoGameEvent;
    [NonSerialized] private MainMenuView _mainMenuView;
#if NAKAMA_ENABLED
    [SerializeField] private NakamaSystem NakamaSystem;
#endif
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
    
    public void ConnectFacebook()
    {
#if FB
        FacebookService.LoginFacebook();
#endif
        // await NakamaSystem.SigninWithFacebook();
    }

    public void DisconnectFacebook()
    {
#if FB
        FacebookService.LogoutFacebook();
#endif
    }

#if NAKAMA_ENABLED
    public async Task<StorageType> ResolveConflict()
    {
        var storageType = await _mainMenuView.ShowSyncConflictPanel();
        return storageType;
    }
#endif

    public void UpdateButton() => _mainMenuView?.UpdateButton();
    
    public void GotoGame()
    {
        GotoGameEvent.Invoke();
    }
}
