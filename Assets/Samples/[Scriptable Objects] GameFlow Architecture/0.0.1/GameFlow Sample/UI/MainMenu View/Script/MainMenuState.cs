using ProjectCore.Events;
using ProjectCore.StateMachine;
using System;
using System.Collections;
using ProjectCore.SocialFeature.Cloud;
using UnityEngine;

[CreateAssetMenu(fileName = "MainMenuState", menuName = "ProjectCore/State Machine/States/MainMenu State")]
public class MainMenuState : State
{
    [SerializeField] private string PrefabName;

    [SerializeField] private GameEvent GotoGameEvent;
    [NonSerialized] private MainMenuView _mainMenuView;
    [SerializeField] private NakamaSystem NakamaSystem;
    public override IEnumerator Execute()
    {
         base.Execute();
        _mainMenuView = Instantiate(Resources.Load<MainMenuView>(PrefabName));

        yield return _mainMenuView.Show(true);
    }

    public override IEnumerator Exit()
    {
        yield return _mainMenuView.Hide(true);
        _mainMenuView = null;
        yield return base.Exit();
    }

    public async void EmailSyncNakama(string email, string password)
    {
        var status = await NakamaSystem.SyncWithEmail(email, password);
        _mainMenuView.ShowErrorText(status);
    }

    public void LogoutNakama()
    {
        NakamaSystem.UnlinkWithEmail();
    }

    public void GotoGame()
    {
        GotoGameEvent.Invoke();
    }
}
