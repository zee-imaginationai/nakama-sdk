﻿using System;
using System.Collections;
using ProjectCore.Application;
using ProjectCore.Events;
using ProjectCore.StateMachine;
using ProjectCore.Variables;
using UnityEngine;
using UnityEngine.SceneManagement;



namespace ProjectCore.Application
{
    [CreateAssetMenu(fileName = "SplashState", menuName = "ProjectCore/State Machine/States/Splash State")]
    public class SplashState : State
    {
        const string APPLICATION_FLOW_CONTROLLER_PATH = "ApplicationFlowController";
        const string UI_SPAWNER_PATH = "UISpawner";
        
       /* [SerializeField] private FullScreenAd InterstitialAd;
        [SerializeField] private FullScreenAd RewardedAd;*/
        /*[SerializeField] private IAPStore IAPStore;*/

        [SerializeField] private int SceneIndex;
        [SerializeField] private Float SceneLoadingProgress;
        [SerializeField] private Float SdkLoadingProgress;
        [SerializeField] private GameEvent HideLoadingView;


        [NonSerialized] private AsyncOperation _sceneLoadingOperation;
        [NonSerialized] private bool socialKitInitialize = false;
        [NonSerialized] private float TimeoutTime = 3;

        private ApplicationFlowController applicationFlowController;


        public override IEnumerator Init(IState listener)
        {
            yield return base.Init(listener);
            
            SceneLoadingProgress.SetValue(0);
          
            socialKitInitialize = false;
            
            //TODO: Initialize GA and FB
        }

        public override IEnumerator Execute()
        {
            yield return base.Execute();
            
            // instantiating flow controller and ui spawner here so that it stays in bootstrap scene
            applicationFlowController =
                Instantiate(Resources.Load<ApplicationFlowController>(APPLICATION_FLOW_CONTROLLER_PATH));
            var uiSpawner = Instantiate(Resources.Load<UISpawner>(UI_SPAWNER_PATH));

            applicationFlowController.UISpawner = uiSpawner;
            uiSpawner.ApplicationFlowController = applicationFlowController;
            
            yield return GameSceneLoading();
        }
        private IEnumerator GameSceneLoading()
        {
            yield return new WaitForSeconds(5);
            _sceneLoadingOperation = SceneManager.LoadSceneAsync(SceneIndex, LoadSceneMode.Additive);

        }
        public override IEnumerator Tick()
        {
            yield return base.Tick();

            while (!_sceneLoadingOperation.isDone)
            {
                SceneLoadingProgress.SetValue(_sceneLoadingOperation.progress);
                yield return null;
            }

            SceneLoadingProgress.SetValue(1.0f);
            

            Scene scene = SceneManager.GetSceneByBuildIndex(SceneIndex);
            SceneManager.SetActiveScene(scene);

            WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);
            float timeStarted = Time.time;
            while(true)
            {
                float timeElapsed = Time.time - timeStarted;
                if (timeElapsed > TimeoutTime)
                {
                    SdkLoadingProgress.SetValue(1f);
                    break;
                }

                if (SdkLoadingProgress >= 1.0f && socialKitInitialize)
                {
                    break;
                }

                yield return waitForSeconds;
            }

            yield return new WaitUntil(() => SdkLoadingProgress.GetValue() >= 1.0f);

            //artificial delay can be removed
            yield return waitForSeconds;
            
            HideLoadingView.Invoke();

            applicationFlowController.Boot();
        }
    }
}