using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class MatchResolution : MonoBehaviour
{
    public float sceneWidth = 10;

    Camera _camera;
    void Start() {
        _camera = GetComponent<Camera>();
    }

    void Update() {
        float unitsPerPixel = Screen.width > Screen.height ? sceneWidth / Screen.width : sceneWidth / Screen.height;
        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
        _camera.orthographicSize = desiredHalfHeight;
    }
}
