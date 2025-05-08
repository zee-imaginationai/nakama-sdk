using System;
using System.Collections;
using System.Collections.Generic;
/*using Analytics;*/
using Sirenix.OdinInspector;
using UnityEngine;
using CustomUtilities;
using ProjectCore.UI;
using ProjectCore.Events;
using ProjectCore.Hud;
using ProjectCore.StateMachine;

namespace ProjectCore.GamePlay
{
    public class GameState : State
    {
        [SerializeField] protected GameHud GameHudPrefab;
        [NonSerialized] protected GameHud _gameHud;

        [FoldoutGroup("Events Raised")][SerializeField] protected GameEvent GameStateStart;
        [FoldoutGroup("Events Raised")][SerializeField] protected GameEvent GameStateResume;
        [FoldoutGroup("Events Raised")][SerializeField] protected GameEvent GameStateExit;
        [FoldoutGroup("Events Raised")][SerializeField] protected GameEvent GameStatePause;
        [FoldoutGroup("Events Raised")][SerializeField] protected GameEvent LevelFail;
        [FoldoutGroup("Events Raised")][SerializeField] protected GameEvent LevelComplete;
        [FoldoutGroup("Events Raised")][SerializeField] protected GameEvent ShowBannerAd;
        [FoldoutGroup("Events Raised")][SerializeField] protected GameEvent HideBannerAd;

        
        
        [NonSerialized] protected bool _failReported = false;
        private GameObject _levelObject;

        

        public override IEnumerator Init(IState listener)
        {
            yield return base.Init(listener);
            yield return listener.CleanupAllPausedStates(this);
            ResetState();
        }

        public override IEnumerator Execute()
        {
            
            yield return base.Execute();
            
            ShowLevelObjects();
        }

        public override IEnumerator Resume()
        {
            yield return base.Resume();
            ShowBannerAd?.Invoke();
            GameStateResume?.Invoke();
            ShowLevelObjects();

        }

        public override IEnumerator Tick()
        {
            yield return base.Tick();
        }

        public override IEnumerator Pause()
        {
            yield return base.Pause();
            
            HideBannerAd?.Invoke();
            GameStatePause?.Invoke();
            HideLevelObjects();
        }

        public override IEnumerator Exit()
        {
            GameStateExit?.Invoke();
            HideBannerAd?.Invoke();
            
            yield return new WaitForSeconds(0.5f); //Delay for Tween Errors/Cleanup

            ResetState();
            yield return base.Exit();
        }

        public override IEnumerator Cleanup()
        {
            ResetState();
            yield return base.Cleanup();
        }

        protected virtual void DestroyLevelObject()
        {
            if (_levelObject != null)
            {
                Destroy(_levelObject.gameObject);
                _levelObject = null;
            }

            if (_gameHud != null)
            {
                Destroy(_gameHud.gameObject);
                _gameHud = null;
            }
        }

        protected virtual void ResetState()
        {
            _failReported = false;

            DestroyLevelObject();
        }

        protected virtual void ShowLevelObjects()
        {
            if (_gameHud!=null)
            {
                _gameHud.Show();
            }
        }

        protected virtual void HideLevelObjects()
        {
            if (_gameHud!=null)
            {
                _gameHud.Hide();
            }
        }

    }
}