using System.Collections;
using ProjectCore.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class UIView : MonoBehaviour
{
    public abstract IEnumerator Show(bool isResumed);
    public abstract IEnumerator Hide(bool shouldDestroy);
    
    #region Debug Functions

    [Button]
    private void DebugShow()
    {
        Show(false);
    }

    [Button]
    private void DebugHide()
    {
        Hide(false);
    }

    #endregion
}
