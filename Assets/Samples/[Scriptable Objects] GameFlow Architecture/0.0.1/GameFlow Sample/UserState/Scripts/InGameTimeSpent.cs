using ProjectCore.Variables;
using UnityEngine;

namespace ProjectCore.Inventory
{
    [CreateAssetMenu(fileName = "v_InGameTimeSpent", menuName = "ProjectCore/UserState/InGameTimeSpent")]
    public class InGameTimeSpent : DBInt
    {
        private float _lastSavedTime = -1;

        #region Public Functions

        public void SaveInGameTimeSpent()
        {
            if (_lastSavedTime == -1) _lastSavedTime = Time.time;
            
            var timePassed = Time.time - _lastSavedTime;
            //Debug.LogError($"Last Save :: {_lastSavedTime} || Current {Time.time} || Passed {timePassed}");
            ApplyChange((int)timePassed);
            _lastSavedTime = Time.time;
        }

        public override int GetValue()
        {
            SaveInGameTimeSpent();
            return base.GetValue();
        }

        #endregion
    }
}