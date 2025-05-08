using System;
using System.Collections;
using ProjectCore.Variables;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace ProjectCore.GamePlay
{
    [CreateAssetMenu(fileName = "NormalGameState", menuName = "ProjectCore/State Machine/States/Normal Game State")]
    [Required]
    public class NormalGameState : GameState
    {
        [SerializeField] private Int GameLevel;

        //Normal game hud reference
        private NormalGameHud _normalGameHud;


        private IEnumerator FreshStart()
        {
            LogLevelStartedEvent();
            yield break;
        }
        public override IEnumerator Resume()
        {

            return base.Resume();
        }
        public override IEnumerator Execute()
        {
           _gameHud = Instantiate(GameHudPrefab);
            _normalGameHud = _gameHud as NormalGameHud;

            yield return base.Execute();
            
            GameStateStart.Invoke();

            yield return FreshStart();

            //TUTORIALS
            // yield return _levelObject.IntroTutorial.Execute();
            
        }


        private void LogLevelStartedEvent()
        {
            var levelNumber = GameLevel;
            string[] levelStructure = {"NormalGame"};
        }
        
    }
}