using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "UIConfig", menuName = "ProjectCore/UI/Configs/UIConfig")]
public class UIConfig : ScriptableObject
{
    public float EaseInTime;
    public float EaseOutTime;
    public float DefaultScreenHeight;
    public Ease EasingIn;
    public Ease EasingOut;

    public float GetOffScreenOffset()
    {
        return (DefaultScreenHeight / (float) Screen.height) * Screen.width;
    }

}
