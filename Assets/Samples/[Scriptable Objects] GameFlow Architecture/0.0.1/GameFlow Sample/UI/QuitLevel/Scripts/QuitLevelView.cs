using TMPro;
using ProjectCore.Events;
using ProjectCore.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectCore.UI
{
    public class QuitLevelView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button BackButton;
        [SerializeField] private Button QuitButton;
        [SerializeField] private UIViewState QuitLevelState;
        [SerializeField] private TextMeshProUGUI LevelNumberText;
        [SerializeField] private TextMeshProUGUI TitleText;
        [SerializeField] private TextMeshProUGUI DescriptionText;
        [SerializeField] private TextMeshProUGUI QuitButtonText;

        [Header("Variables")]
        [SerializeField] private DBInt GameLevel;
        [SerializeField] private Bool DailyChallengeHasStarted;
        [SerializeField] private Bool WasDCStartedFromFSP;
        [SerializeField] private Bool IsInfiniteLivesBoostActive;
        [SerializeField] private DBInt CompletedCascadeIndex;
        [SerializeField] private Int CurrentCascadeIndex;
        [SerializeField] private Int TilesCollected;
        

        [Header("Game Objects")]
        [SerializeField] private GameObject HeartObject;
        [SerializeField] private GameObject EventObject;
        [SerializeField] private GameObject FailPlantObject;
        [SerializeField] private GameObject QuitButtonBrokenHeartObject;
        [SerializeField] private GameObject QuitButtonInfiniteHeartObject;
        [SerializeField] private GameObject LeftHeart;
        [SerializeField] private GameObject RightHeart;

        [Header("Other References")]
        [SerializeField] private Bool NoAdsBoosterActive;
        [SerializeField] private Animator HeartBreakAnimator;


        #region Mono Methods

        private void OnEnable()
        {
            SetupLevelNumberText();
            SetDescriptionText();
            TryEventFail();
            RegisterListeners();
        }

        private void OnDisable()
        {
            UnregisterListeners();
        }

        #endregion

        #region Private Methods

        private void SetDescriptionText()
        {
            FailPlantObject.SetActive(IsInfiniteLivesBoostActive && DailyChallengeHasStarted);
            HeartObject.SetActive(!IsInfiniteLivesBoostActive);
            QuitButtonBrokenHeartObject.SetActive(!NoAdsBoosterActive);
            LeftHeart.SetActive(!NoAdsBoosterActive);
            RightHeart.SetActive(!NoAdsBoosterActive);
            HeartBreakAnimator.enabled = !NoAdsBoosterActive;
            QuitButtonBrokenHeartObject.SetActive(!IsInfiniteLivesBoostActive);
            QuitButtonInfiniteHeartObject.SetActive(IsInfiniteLivesBoostActive);

            if (IsInfiniteLivesBoostActive)
            {
                DescriptionText.text = "You will lose your progress!";
            }

            else
            {
                DescriptionText.text = "You will lose a life and your progress!";
            }
        }

        private void SetupLevelNumberText()
        {
            if (DailyChallengeHasStarted.GetValue())
            {
                LevelNumberText.gameObject.SetActive(false);
            }

            else
                LevelNumberText.text = $"LEVEL {GameLevel.GetValue()}";
        }

        private void TryEventFail()
        {
            int inLevelEventTilesAmount = 0;


        }

        #endregion

        #region Buttons Register & Unregister

        private void RegisterListeners()
        {
            BackButton.onClick.AddListener(BackButtonPressed);
            QuitButton.onClick.AddListener(QuitButtonPressed);
        }

        private void UnregisterListeners()
        {
            BackButton.onClick.RemoveListener(BackButtonPressed);
            QuitButton.onClick.RemoveListener(QuitButtonPressed);
        }

        #endregion

        #region Button Callbacks

        private void BackButtonPressed()
        {
            QuitLevelState.CloseView(UICloseReasons.ResumeGame);
        }

        private void QuitButtonPressed()
        {
            if (DailyChallengeHasStarted)
            {
                if (WasDCStartedFromFSP)
                {
                    WasDCStartedFromFSP.SetValue(false);
                    QuitLevelState.CloseView(UICloseReasons.Game);
                }
                else
                {
                    QuitLevelState.CloseView(UICloseReasons.Home);
                }
            }
            else
            {
                CurrentCascadeIndex.SetValue(0);
                CompletedCascadeIndex.SetValue(0);
                QuitLevelState.CloseView(UICloseReasons.Game);
            }
        }

        #endregion
    }
}