using Sirenix.OdinInspector;
using UnityEngine;
using System;
using ProjectCore.Events;
using ProjectCore.StateMachine;
using ProjectCore.UI;

namespace ProjectCore.Application
{
    [Searchable]
    public class UISpawner : MonoBehaviour
    {
        [HideInInspector] public ApplicationFlowController ApplicationFlowController;

        [SerializeField] private FiniteStateMachine StateMachine;

        [FoldoutGroup("Coin Visualization", false)] [SerializeField]
        private GameEvent StartCoinAnimation;
        

        [FoldoutGroup("Settings", false)]
        [SerializeField]
        private UIViewTransition SettingsTransition;

        [FoldoutGroup("Settings", false)]
        [SerializeField]
        private GameEventWithBool ShowSettings;

        [FoldoutGroup("Level Complete", false)]
        [SerializeField] private GameEvent ShowLevelComplete;
        [FoldoutGroup("Level Complete", false)]
        [SerializeField]
        private UIViewTransition LevelCompleteTransition;

        [FoldoutGroup("Level Fail", false)]
        [SerializeField]
        private UIViewTransition LevelFailTransition;
        [FoldoutGroup("Level Fail", false)]
        [SerializeField]
        private GameEvent ShowLevelFail;

        [FoldoutGroup("Resume Game View", false)]
        [SerializeField]
        private UIViewTransition ResumeGameViewTransition;

        [FoldoutGroup("Resume Game View", false)]
        [SerializeField]
        private UIViewTransition ResumeGameViewOnAppResumeTransition;

       

        
        
        
        //openView
        [SerializeField] private UIViewTransition OpenViewTransition;
        [SerializeField] private GameEvent ShowOpenView;

        
        private Camera _camera;

        private Camera Camera
        {
            get
            {
                if (_camera == null)
                {
                    _camera = Camera.main;
                }

                return _camera;
            }
        }

        private void OnEnable()
        {
            
            ShowOpenView.Handler += OnShowOpenView;
            ShowLevelComplete.Handler += OnShowLevelCompleteView;
            ShowLevelFail.Handler += OnShowLevelFailView;
            return;

            
        }

        private void OnDisable()
        {
            

            ShowOpenView.Handler -= OnShowOpenView;
            ShowLevelComplete.Handler -= OnShowLevelCompleteView;
            ShowLevelFail.Handler -= OnShowLevelFailView;
            return; 
        }


        public void OnShowLevelFailView()
        {
            LevelFailTransition.Camera = Camera;
            StateMachine.Transition(LevelFailTransition);
        }
       

        public void OnShowLevelCompleteView()
        {
            
            LevelCompleteTransition.Camera = Camera;
            StateMachine.Transition(LevelCompleteTransition);
        }


        public void ShowResumeGameViewOnAppResume()
        {
            ResumeGameViewOnAppResumeTransition.Camera = Camera;
            ResumeGameViewOnAppResumeTransition.ViewSourceReasonClose = UICloseReasons.ResumeAny;
            StateMachine.Transition(ResumeGameViewOnAppResumeTransition);
        }

       
      
       
        #region ShowView
        

        private void OnShowOpenView()
        {
            OpenViewTransition.Camera = Camera;
            StateMachine.Transition(OpenViewTransition);
        }
        #endregion

    }
}