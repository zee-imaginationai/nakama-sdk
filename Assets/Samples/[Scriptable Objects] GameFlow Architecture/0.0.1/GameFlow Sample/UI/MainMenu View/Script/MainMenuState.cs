using ProjectCore.Events;
using ProjectCore.StateMachine;
using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MainMenuState", menuName = "ProjectCore/State Machine/States/MainMenu State")]
public class MainMenuState : State
{
    [SerializeField] private string PrefabName;

    [SerializeField] private GameEvent GotoGameEvent;
    [NonSerialized] private MainMenuView _mainMenuView;
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

    public void GotoGame()
    {
        GotoGameEvent.Invoke();
    }
}
