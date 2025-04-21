using ProjectCore.Events;
using ProjectCore.StateMachine;
using System;
using System.Collections;
using Nakama;
using ProjectCore.SocialFeature;
using ProjectCore.SocialFeature.Cloud.Internal;
using UnityEngine;

[CreateAssetMenu(fileName = "MainMenuState", menuName = "ProjectCore/State Machine/States/MainMenu State")]
public class MainMenuState : State
{
    [SerializeField] private string PrefabName;

    [SerializeField] private GameEvent GotoGameEvent;
    [NonSerialized] private MainMenuView _mainMenuView;
    [SerializeField] private CloudSyncSystem CloudSyncSystem;
    [SerializeField] private UserProfileService UserProfileService;
    [SerializeField] private CloudDBString ConflictString;
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

    public string GetConflictString()
    {
        return ConflictString.GetValue();
    }

    public void UpdateConflictString(string value)
    {
        ConflictString.SetValue(value);
        UserProfileService.SaveUserData();
    }

    public async void SignupWithEmail(string email, string password)
    {
        await CloudSyncSystem.SignupWithEmail(email, password, OnConnectionWithEmail);
    }

    public async void LoginWithEmail(string email, string password)
    {
        await CloudSyncSystem.SigninWithEmail(email, password, OnConnectionWithEmail);
    }

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
        await CloudSyncSystem.SignOutFromEmail(OnLogout);
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
