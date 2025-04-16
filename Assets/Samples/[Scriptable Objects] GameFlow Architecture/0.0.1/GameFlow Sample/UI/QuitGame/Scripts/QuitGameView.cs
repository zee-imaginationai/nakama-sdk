using UnityEngine;
using UnityEngine.UI;

namespace ProjectCore.UI
{
    public class QuitGameView : MonoBehaviour
    {
        [SerializeField] private Button CloseButton;
        [SerializeField] private Button QuitButton;
        [SerializeField] private UIViewState QuitViewState;

        #region Mono Methods

        private void OnEnable()
        {
            RegisterListeners();
        }

        private void OnDisable()
        {
            UnregisterListeners();
        }

        #endregion

        #region Register & Unregister Listeners

        private void RegisterListeners()
        {
            CloseButton.onClick.AddListener(CloseButtonPressed);
            QuitButton.onClick.AddListener(QuitButtonPressed);
        }

        private void UnregisterListeners()
        {
            CloseButton.onClick.RemoveListener(CloseButtonPressed);
            QuitButton.onClick.RemoveListener(QuitButtonPressed);
        }

        #endregion

        #region Button Callbacks

        private void QuitButtonPressed()
        {
            UnityEngine.Application.Quit();
        }

        private void CloseButtonPressed()
        {
            QuitViewState.CloseView();
        }

        #endregion
    }
}