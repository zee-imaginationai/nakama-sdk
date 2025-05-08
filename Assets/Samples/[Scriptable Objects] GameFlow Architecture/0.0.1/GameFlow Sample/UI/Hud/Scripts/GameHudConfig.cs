using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectCore.Hud
{
    [CreateAssetMenu(fileName = "GameHudConfig", menuName = "ProjectCore/UI/Configs/GameHudConfig")]
    public class GameHudConfig : ScriptableObject
    {
        public float AnimationDuration;
        public float HeaderOffScreenPosition;
        public float FooterOffScreenPosition;
        
        public float LeftSideBarOffScreenPosition;
        public float RightSideBarOffScreenPosition;
    }
}