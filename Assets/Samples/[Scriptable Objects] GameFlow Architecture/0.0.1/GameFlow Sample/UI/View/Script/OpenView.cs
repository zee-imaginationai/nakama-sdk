using UnityEngine;
using UnityEngine.UI;

public class OpenView : UiPanelInAndOut
{
    [SerializeField] private Button CloseButton;

    [SerializeField] private OpenViewState OpenViewState;
    private void OnEnable()
    {
        CloseButton.onClick.AddListener(OnCloseButtonPressed);
    }

    private void OnDisable()
    {
        CloseButton.onClick.RemoveListener(OnCloseButtonPressed);
    }
    private void OnCloseButtonPressed()
    {
        OpenViewState.CloseView(ProjectCore.UI.UICloseReasons.Home);
    }

   
}
