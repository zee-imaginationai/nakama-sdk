using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

using ProjectCore.UI;

using ProjectCore.StateMachine;
using ProjectCore.Events;

[CreateAssetMenu(fileName = "UIViewState", menuName = "ProjectCore/State Machine/States/UI View State")]
public class UIViewState : State
{
    [System.NonSerialized, HideInInspector]
    public Camera RenderCamera;

    [System.NonSerialized, HideInInspector]
    public UICloseReasons ViewSourceReasonClose;

    [SerializeField] protected GameEventWithInt EventWithCloseReason;
    [SerializeField] protected string prefabName;

    private string _originalPrefabName;

    

    protected UIView View;

    [Button(ButtonSizes.Medium)]
    public virtual void CloseView()
    {
        CloseView(ViewSourceReasonClose);
    }

    [Button(ButtonSizes.Medium)]
    public virtual void CloseView (UICloseReasons reasons)
    {
        if (EventWithCloseReason != null)
        {
            EventWithCloseReason.Raise((int)reasons);
        }
    }

    public override IEnumerator Init(IState listener)
    {
        yield return base.Init(listener);

        GameObject gameObject = UIPanelFactory.CreatePanel(prefabName, RenderCamera);
        View = gameObject.GetComponent<UIView>();
    }

    public override IEnumerator Execute()
    {
        yield return base.Execute();
        yield return View.Show(false);
        
    }

    public override IEnumerator Exit()
    {
        yield return View.Hide(true);
        View = null;
        yield return base.Exit();
    }

    public override IEnumerator Pause()
    {
        yield return View.Hide(false);
        yield return base.Pause();
    }

    public override IEnumerator Resume()
    {
        if (View == null)
        {
            GameObject gameObject = UIPanelFactory.CreatePanel(prefabName, RenderCamera);
            View = gameObject.GetComponent<UIView>();
        }

        yield return View.Show(true);
        yield return base.Resume();
    }

    public override IEnumerator Cleanup()
    {
        if (View != null) 
        {
            Destroy(View.gameObject);
        }

        View = null;

        yield return base.Cleanup();
    }


  
    public void SetPrefabName(string prefabName)
    {
        this.prefabName = prefabName;
    }

}
