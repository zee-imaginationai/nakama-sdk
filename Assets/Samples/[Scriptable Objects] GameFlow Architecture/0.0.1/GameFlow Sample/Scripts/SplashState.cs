using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using ExtensionMethods;
#if NAKAMA_ENABLED
using ProjectCore.Integrations.NakamaServer;
#endif
using ProjectCore.Events;
using ProjectCore.Integrations.FacebookService;
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
        /*[SerializeField] private IAPStore IAPStopanel-3-32re;*/

        [SerializeField] private int SceneIndex;
        [SerializeField] private Float SceneLoadingProgress;
        [SerializeField] private Float SdkLoadingProgress;
        [SerializeField] private GameEvent HideLoadingView;


        [NonSerialized] private AsyncOperation _sceneLoadingOperation;
        [NonSerialized] private bool socialKitInitialize = false;
        [NonSerialized] private float TimeoutTime = 10;

#if NAKAMA_ENABLED
        [SerializeField] private NakamaSystem NakamaSystem;
#endif
        
        [SerializeField] private Float CloudServiceProgress;
        [SerializeField] private FacebookService FacebookService;
        
        private ApplicationFlowController applicationFlowController;
        
        private CancellationTokenSource _cancellationTokenSource;

        public override IEnumerator Init(IState listener)
        {
            yield return base.Init(listener);
            
            SceneLoadingProgress.SetValue(0);
            
            socialKitInitialize = false;

            //TODO: Initialize GA and FB
#if NAKAMA_ENABLED
            NakamaSystem.Initialize();
#endif
            FacebookService.Initialize();
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
            
#if NAKAMA_ENABLED
            var task = NakamaSystem.AuthenticateNakama(_cancellationTokenSource.RefreshToken());
#endif
            
            WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);
            float timeStarted = Time.time;
            while(true)
            {
                float timeElapsed = Time.time - timeStarted;
                if (timeElapsed > TimeoutTime)
                {
                    _cancellationTokenSource.Cancel();
                    SdkLoadingProgress.SetValue(1f);
                    CloudServiceProgress.SetValue(1f);
                    break;
                }

                if ((SdkLoadingProgress >= 1.0f && CloudServiceProgress >= 1.0f)
#if NAKAMA_ENABLED
                    || IsTaskCompleted(task))
#else
                )
#endif
                {
                    break;
                }

                yield return waitForSeconds;
            }

            yield return new WaitUntil(() => SdkLoadingProgress.GetValue() >= 1.0f 
                                             && CloudServiceProgress.GetValue() >= 1.0f);
            
#if NAKAMA_ENABLED
            Debug.LogError($"Task : {task.Status}");
#endif

            //artificial delay can be removed
            yield return waitForSeconds;
            
            HideLoadingView.Invoke();

            applicationFlowController.Boot();
        }
        
        private bool IsTaskCompleted(Task task)
        {
            return task.IsCompleted || task.IsFaulted || task.IsCanceled;
        }
    }
}