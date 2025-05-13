using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelFactory
{
	public static GameObject CreatePanel(string panelName, Camera renderCamera)
	{
		//Debug.Log($"[UI][Spawning]{panelName}");
		GameObject panel = MonoBehaviour.Instantiate(Resources.Load(panelName)) as GameObject;
		Canvas panelCanvas = panel.GetComponent<Canvas>();
		panelCanvas.worldCamera = renderCamera;
		return panel;
	}
}
