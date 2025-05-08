using System.Collections;
using UnityEngine;

using ProjectCore.StateMachine;

using ProjectCore.UI;


[CreateAssetMenu(fileName ="ViewTransition", menuName ="ProjectCore/State Machine/Transitions/View Transitions")]
public class UIViewTransition : Transition
{
    [System.NonSerialized, HideInInspector]
    public UICloseReasons ViewSourceReasonClose;

    [System.NonSerialized, HideInInspector]
    public Camera Camera;

    public override IEnumerator Execute()
    {
        yield return base.Execute();

        UIViewState state = ToState as UIViewState;
        state.ViewSourceReasonClose = ViewSourceReasonClose;
        state.RenderCamera = Camera;
    }
}
