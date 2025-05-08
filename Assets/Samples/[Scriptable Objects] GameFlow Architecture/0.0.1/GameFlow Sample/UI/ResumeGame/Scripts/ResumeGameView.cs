using UnityEngine;

using ProjectCore.UI;
using ProjectCore.Events;

public class ResumeGameView : MonoBehaviour
{
    [SerializeField] private UIViewState ResumeGameViewState;

    public void GotoHome()
    {
        ResumeGameViewState.CloseView(UICloseReasons.Home);
    }

    public void ContinueGame()
    {
        ResumeGameViewState.CloseView();
    }
}
