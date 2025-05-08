using System;
using ProjectCore.Events;
using ProjectCore.StateMachine;
using ProjectCore.Variables;
using UnityEngine;
using ProjectCore.Inventory;
using ProjectCore.TimeMachine;
using System.Collections;
using CustomUtilities;

public class ApplicationBase : MonoBehaviour
{
    [SerializeField] private int IOSTargetFrameRate = 60;
    [SerializeField] private int AndroidTargetFrameRate = 60;

    [SerializeField] private FiniteStateMachine ApplicationStateMachine;
    [SerializeField] private Float SdkLoadingProgress;
    [SerializeField] private TimeMachine ApplicationTimeMachine;

    [SerializeField] private GameEvent AppPaused;
    [SerializeField] private GameEvent AppResumed;
    [SerializeField] private DBInt AppPausedTime;

    [Header("InGame Time")]
    [SerializeField] private InGameTimeSpent InGameTimeSpent;


    private Coroutine _stateMachineRoutine = null;
    private Coroutine _timeMachineRoutine;
    private bool _appStarted = false;
    private bool _appPaused = false;

    public void SdkLoadingCompleted()
    {
        SdkLoadingProgress.SetValue(1.0f);
    }

    #region Life Cycle
    // Start is called before the first frame update
    private IEnumerator Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Application.targetFrameRate = AndroidTargetFrameRate;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Application.targetFrameRate = IOSTargetFrameRate;
        }

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
       

        yield return new WaitForSeconds(1);

        _appStarted = true;

        _timeMachineRoutine = StartCoroutine(ApplicationTimeMachine.Tick());
        _stateMachineRoutine = StartCoroutine(ApplicationStateMachine.Tick());
        yield return new WaitForSeconds(1);


        InGameTimeSpent.SaveInGameTimeSpent();

        // Initialize Firebase and Ads Here

    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            ApplicationResumed();
        }
        else
        {
            ApplicationPaused();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            ApplicationPaused();
        }
        else
        {
            ApplicationResumed();
        }
    }

    private void OnApplicationQuit()
    {
        ApplicationPaused();
    }

#endregion

    private void ApplicationPaused ()
    {
        InGameTimeSpent.SaveInGameTimeSpent();
        if(_appStarted && !_appPaused)
        {
            _appPaused = true;
            AppPausedTime.SetValue((int)DateTimeOffset.Now.ToUnixTimeSeconds());
            AppPaused.Invoke();
        }
    }

    private void ApplicationResumed ()
    {
        InGameTimeSpent.SaveInGameTimeSpent();
        if (_appStarted)
        {

            if (_appPaused)
            {
                _appPaused = false;
                AppResumed.Invoke();
            }
        }
    }
}
