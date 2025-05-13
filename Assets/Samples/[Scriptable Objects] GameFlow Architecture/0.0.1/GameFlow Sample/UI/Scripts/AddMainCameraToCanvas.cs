using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddMainCameraToCanvas : MonoBehaviour
{
    [SerializeField] private Canvas Canvas;

    private static Camera _camera;
    private Camera Camera
    {
        get
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            return _camera;
        }
    }

    private void OnEnable()
    {
        Canvas.worldCamera = Camera;
    }
}
