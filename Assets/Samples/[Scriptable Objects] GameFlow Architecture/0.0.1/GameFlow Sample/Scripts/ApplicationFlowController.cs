using System;
using System.Collections;
using ProjectCore.Events;
using ProjectCore.GamePlay;
using ProjectCore.StateMachine;
using ProjectCore.UI;
using ProjectCore.Variables;
using UnityEngine;

namespace ProjectCore.Application
{
    public class ApplicationFlowController : MonoBehaviour
    {
        const int RESUME_FLOW_RESUME_TIME = 15;
        const int RESUME_FLOW_MM_TIME = 86000;

        [HideInInspector] public UISpawner UISpawner;

        [SerializeField] private FiniteStateMachine StateMachine;

        [Header("Main Menu")]
        [SerializeField] private Transition MainMenuTransition;
        [SerializeField] private GameEvent GotoMainMenu;

        [Header("Game")]
        [SerializeField] private Transition GameStateTransition;
        [SerializeField] private NormalGameState NormalGameState;
        [SerializeField] private Int GameLevel;
        [SerializeField] private GameEvent GotoGame;
        [SerializeField] private GameEvent LevelComplete;
        [SerializeField] private GameEvent LevelFail;

        [Header("Pause/Resume Game")]
        [SerializeField] private DBInt AppPausedTime;
        [SerializeField] private GameEvent AppPaused;
        [SerializeField] private GameEvent AppResumed;
        [SerializeField] private GameEventWithInt ResumeGameViewClose;
        [SerializeField] private GameEventWithInt ResumeGameViewOnAppResumeViewClose;
       
       
        [SerializeField] private GameEventWithInt OpenViewClose;

        private bool _shouldResumeOnAppResume = true;

        public void Boot ()
        {
            StartAppFlowController();
        }

    
        
        //SettingBox View
        private void OnSettingBoxViewClose(int value)
        {
            OnResumePreviousState();
        }
        
        private void StartAppFlowController ()
        {
            int deltaTime = GetPauseToResumeDeltaTime();

             OnGotoMainMenu();
        }

        private void OnAppResumed()
        {
            return;
            int deltaTime = GetPauseToResumeDeltaTime();
            if (_shouldResumeOnAppResume)
            {
                bool gameStateStatus = StateMachine.RunningState == NormalGameState;
                bool challengeGameState = StateMachine.RunningState is GameState;
             /*   FirebaseRemoteData.FetchDataAsync();*/
 

                if (gameStateStatus)
                {
                    if (deltaTime > RESUME_FLOW_MM_TIME)
                    {
                        OnGotoMainMenu();
                    }
                    else if (deltaTime > RESUME_FLOW_RESUME_TIME)
                    {
                        UISpawner.ShowResumeGameViewOnAppResume();
                    }
                }
                else if (challengeGameState)
                {
                    if (deltaTime > RESUME_FLOW_RESUME_TIME)
                    {
                        UISpawner.ShowResumeGameViewOnAppResume();
                    }
                }
            }
        }

        private void OnAppPaused()
        {
            // check if ad is showing
           // _shouldResumeOnAppResume = !/*IAPStore.IAPInProgress ||*/ /*RewardedAd.AdShowing || InterstitialAd.AdShowing*/ 
            if (_shouldResumeOnAppResume)
            {
                bool gameStateStatus = StateMachine.RunningState == NormalGameState;
                bool challengeGameState = StateMachine.RunningState is GameState;
                _shouldResumeOnAppResume = gameStateStatus || challengeGameState;

                if (gameStateStatus)
                {
                    // NormalGameState.SaveState();
                }
            }

           
        }


        private int GetPauseToResumeDeltaTime()
        {
            return AppPausedTime == 0 ? 0 : Mathf.Abs((int)DateTimeOffset.Now.ToUnixTimeSeconds() - AppPausedTime);
        }

        //TODO need to change this --- putting it up here for more attention
        private void OnResumePreviousState()
        {
            StateMachine.ShouldResumePreviousState();
        }

        private IEnumerator MainMenuInvoke()
        {
            yield return new WaitForSeconds(1f);
            GotoMainMenu.Invoke();
        }

        //INTENTIONALLY NOT ADDED UNSUB FOR THESE EVENTS
        private void Awake()
        {
            GotoGame.Handler += OnGotoGame;
            GotoMainMenu.Handler += OnGotoMainMenu;

            OpenViewClose.Handler += OnOpenViewClsoe;
            return;
        }

        private void OnGotoMainMenu()
        {
            Debug.LogError($"OnGotoMainMenu Called");
            StateMachine.Transition(MainMenuTransition);
        }

        public void OnGotoGame()
        {
            StateMachine.Transition(GameStateTransition);
        }
        private void OnLevelComplete()
        {
            GameLevel.ApplyChange(1);
          
            UISpawner.OnShowLevelCompleteView();
        }

        private void OnLevelFail()
        {
            UISpawner.OnShowLevelFailView();
        }
        

        private void OnLevelFailViewClose(int value)
        {
     
            UICloseReasons reason = (UICloseReasons)value;
            switch (reason)
            {
                case UICloseReasons.Home:
                    
                    // FullScreenPlacementState.Show(GameLevel, FullScreenPlacementSource.LevelFailClose, reason);
                    GotoMainMenu.Invoke();
                    break;
                case UICloseReasons.Game:
                   GotoGame.Invoke();
                    break;

                case UICloseReasons.Revive:
                    //Set Revive Bool True Here
                    // ShouldReviveGameOnResume.SetValue(true);
                    OnResumePreviousState();
                    break;

                case UICloseReasons.SkipLevel:
                    // StarsAttained.SetValue(3);
                    // GameLevel.ApplyChange(1);
                    // OnGotoGame();
                    OnLevelComplete(); //To Test
                    break;
            }
        }

        
    //Setting View Close
        private void OnSettingsViewClose(int value)
        {
            UICloseReasons reason = (UICloseReasons)value;
            switch (reason)
            {
                case UICloseReasons.ResumeGame:
                    // FullScreenPlacementState.Show(GameLevel, FullScreenPlacementSource.InGameSettingsClose, reason);
                    break;

                case UICloseReasons.Home:
                    GotoMainMenu.Invoke();
                    break;

                case UICloseReasons.Game: //restart
                    /*Analytics.LogLevelRestarted();*/
                  GotoGame.Invoke();
                    break;

                case UICloseReasons.SkipLevel:
                    // StarsAttained.SetValue(3);
                    OnLevelComplete();
                    break;
                
            }
        }

        private void OnAppRatingViewClose(int value)
        {
            UICloseReasons reason = (UICloseReasons)value;
            switch (reason)
            {
                // case UICloseReasons.FullScreenPlacement:
                //     FullScreenPlacementState.ViewClosed();
                //     break;

                default:
                    OnResumePreviousState();
                    break;
            }
        }

        private void OnRemoveAdsViewClose(int value)
        {
            UICloseReasons reason = (UICloseReasons)value;
            switch (reason)
            {
                // case UICloseReasons.FullScreenPlacement:
                //     FullScreenPlacementState.ViewClosed();
                //     break;

                default:
                    OnResumePreviousState();
                    break;
            }
        }

        private void OnQuitLevelViewClose(int value)
        {
            UICloseReasons reason = (UICloseReasons)value;
            // EnergySystem.LimitedEnergyHelper.ResetEnergyDeductionFlag();
            switch (reason)
            {
                case UICloseReasons.ResumeGame:
                    OnResumePreviousState();
                    break;

                case UICloseReasons.Home:
                    OnGotoMainMenu();
                    break;

                case UICloseReasons.Game:
                    OnGotoGame();
                    break;
            }
        }

        private void OnLimitedOfferViewClose(int value)
        {
            UICloseReasons reason = (UICloseReasons)value;
            switch (reason)
            {
                // case UICloseReasons.FullScreenPlacement:
                //     FullScreenPlacementState.ViewClosed();
                //     break;

                default:
                    OnResumePreviousState();
                    break;
            }
            
        }

      

       

       

       

        private void OnCloseQuitGameView(int value)
        {
            UICloseReasons reason = (UICloseReasons) value;

            switch (reason)
            {
                // case UICloseReasons.Home:
                //     OnGotoMainMenu();
                //     break;
                
                // case UICloseReasons.ResumeHome:
                //     OnResumePreviousState(); 
                //     break;
                
                default:
                    OnGotoMainMenu();
                    break;
            }
        }
        private void OnOpenViewClsoe(int value)
        {
            UICloseReasons reasons = (UICloseReasons)value;

            switch (reasons)
            {
                case UICloseReasons.Home:
                    OnGotoMainMenu();
                    break;
            }
        }
    }
}