﻿/**
 * Copyright (c) 2017-present, PFW Contributors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software distributed under the License is
 * distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See
 * the License for the specific language governing permissions and limitations under the License.
 */

using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/**
 * Sliding camera is our main RTS cam. It is wargame-like and provides almost entirely free movement. Zooming in goes toward the cursor ("sliding"), zooming out moves back and up at a fixed angle. The camera faces up slightly when zoomed all the way into the ground, and tries to restore its facing when zoomed out again.
 *
 * Restrictions:
 * - Players can't look too far up or down.
 * - There are minimal and maximal altitudes to prevent going below the level.
 *
 * TODO:
 * - Maybe prevent the camera from clipping into units.
 * - A/B test values for a better feel.
 */
public class SlidingCameraBehaviour : MonoBehaviour
{
    [Header("Translational Movement")]
    [SerializeField]
    private float _panSpeed = 50f ;
    [SerializeField]
    private float _panLerpSpeed = 100f;
    [SerializeField]
    private float _borderPanningOffset = 2; // Pixels
    [SerializeField]
    private float _borderPanningCornerSize = 200; // Pixels
    [SerializeField]
    private float _maxCameraHorizontalDistanceFromTerrain =
            50000f;

    private Image _cornerArrowBottomLeft;
    private Image _cornerArrowBottomRight;
    private Image _cornerArrowTopLeft;
    private Image _cornerArrowTopRight;
    private Image _sideArrowLeft;
    private Image _sideArrowRight;
    private Image _sideArrowTop;
    private Image _sideArrowBottom;

    [Header("Rotational Movement")]
    [SerializeField]
    private float _horizontalRotationSpeed = 600f;
    [SerializeField]
    private float _verticalRotationSpeed = 600f;
    [SerializeField]
    private float _rotLerpSpeed = 10f;
    [SerializeField]
    private float _maxCameraAngle = 85f;
    [SerializeField]
    private float _minCameraAngle = 5f;

    [Header("Zoom Level")]
    [SerializeField]
    private float _zoomSpeed = 5000f ;
    [SerializeField]
    private float _zoomTiltSpeed = 4f;
    [SerializeField]
    private float _minAltitude = 1.0f ;
    [SerializeField]
    private float _tiltThreshold = 2f;
    [SerializeField]
    private float _maxAltitude = 20000f ;
    [SerializeField]
    private float _heightSpeedScaling = 0.75f;
    [SerializeField]
    private float _zoomOutAngle = 45f;

    private Vector3 _zoomOutDirection;

    // We store the camera facing and reapply it every LateUpdate() for simplicity:
    public float _rotateX;
    public float _rotateY;

    // Leftover translations from zoom and pan:
    private float _translateX;
    private float _translateZ;
    private float _leftoverZoom = 0f;
    private Vector3 _zoomDestination;

    // All planned transforms are actually applied to a target object, which the camera then lerps to.
    public Vector3 _targetPosition;

    private Camera _cam;

    private Terrain _terrain;
    /// <summary>
    /// 是否旋转
    /// </summary>
    public bool isRotateGo;

    [Serializable]
    private struct TerrainMaterial
    {
#pragma warning disable 0649
        public Material Material;
       // public MicroSplatPropData PerTextureData;
        //public MicroSplatKeywords Keywords;
        public float MaxAltitude;
#pragma warning restore 0649
    }

    [Header("Microsplat Terrain Materials")]
    [SerializeField]
    [Tooltip("Must be sorted in ascending order. The first material whose max altitude "
        + "is higher than the current camera altitude will be applied to the terrain.")]
    List<TerrainMaterial> _terrainMaterials = null;
   // [SerializeField]
    //private MicroSplatTerrain _microSplatTerrain = null;

    private void Awake()
    {
        _terrain = Terrain.activeTerrain;

        //if (_microSplatTerrain == null) {
        //    _microSplatTerrain = _terrain.GetComponent<MicroSplatTerrain>();
        //}

        //if (_microSplatTerrain == null) {
        //    throw new Exception("Camera not set up correctly, microsplat reference missing!");
        //}

        //if (_terrainMaterials == null || _terrainMaterials.Count == 0) {
        //    throw new Exception("Camera not set up correctly, terrain materials missing!");
        //}

        _cam = GetComponent<Camera>();
    }

    private void Start()
    {
        _rotateX = transform.eulerAngles.x;
        _rotateY = transform.eulerAngles.y;
        _targetPosition = transform.position;

        _zoomOutDirection = Quaternion.AngleAxis(_zoomOutAngle, Vector3.right) * Vector3.back;

        //_cornerArrowBottomLeft = GameObject.Find("PanningArrowBottomLeft").GetComponent<Image>();
        //if (_cornerArrowBottomLeft == null)
        //    throw new Exception("No cornerArrowBottomLeft specified!");

        //_cornerArrowBottomRight = GameObject.Find("PanningArrowBottomRight").GetComponent<Image>();
        //if (_cornerArrowBottomRight == null)
        //    throw new Exception("No cornerArrowBottomRight specified!");

        //_cornerArrowTopLeft = GameObject.Find("PanningArrowTopLeft").GetComponent<Image>();
        //if (_cornerArrowTopLeft == null)
        //    throw new Exception("No cornerArrowTopLeft specified!");

        //_cornerArrowTopRight = GameObject.Find("PanningArrowTopRight").GetComponent<Image>();
        //if (_cornerArrowTopRight == null)
        //    throw new Exception("No cornerArrowTopRight specified!");

        //_sideArrowLeft = GameObject.Find("PanningArrowLeft").GetComponent<Image>();
        //if (_sideArrowLeft == null)
        //    throw new Exception("No sideArrowLeft specified!");

        //_sideArrowRight = GameObject.Find("PanningArrowRight").GetComponent<Image>();
        //if (_sideArrowRight == null)
        //    throw new Exception("No sideArrowRight specified!");

        //_sideArrowTop = GameObject.Find("PanningArrowTop").GetComponent<Image>();
        //if (_sideArrowTop == null)
        //    throw new Exception("No sideArrowTop specified!");

        //_sideArrowBottom = GameObject.Find("PanningArrowBottom").GetComponent<Image>();
        //if (_sideArrowBottom == null)
        //    throw new Exception("No sideArrowBottom specified!");
    }

    // Update() only plans movement; position/rotation are directly changed in LateUpdate().
    private void Update()
    {
        if (!CheckMouseInScreen()) return;
        // Camera panning:
        _translateX += CustomInput.Horizontal * GetScaledPanSpeed();
        _translateZ += CustomInput.Vertical * GetScaledPanSpeed();

        if (CustomInput.Horizontal == 0 && CustomInput.Vertical == 0) {
            //Try border panning with mouse
            PanFromScreenBorder();
        } else {
            //SetPanningCursor(ScreenCorner.None);
        }

        AimedZoom();

        if (Mouse.current.rightButton.isPressed && isRotateGo==false) {
            RotateCamera();
        }
    }

    public void SetTargetPosition(Vector3 target)
    {
        _targetPosition = target;
        _translateX = 0;
        _translateZ = 0;
        _leftoverZoom = 0;
    }

    public void LookAt(Vector3 target)
    {

        var toTarget = target - _targetPosition;
        var rotFromX =
                Vector3.Angle(
                        Vector3.ProjectOnPlane(transform.forward, new Vector3(1,0,0)),
                        Vector3.ProjectOnPlane(toTarget, new Vector3(1, 0, 0)));
        var rotFromY =
                Vector3.Angle(
                        Vector3.ProjectOnPlane(transform.forward, new Vector3(0, 1, 0)),
                        Vector3.ProjectOnPlane(toTarget, new Vector3(0, 1, 0)));
        _rotateX += rotFromX;
        _rotateY += rotFromY;
    }

    // If we allow the camera to get to height = 0 we would
    // need special cases for the height scaling.
    private float GetScaledPanSpeed()
    {
        //if (SceneManager.GetActiveScene().name == ScenceName.Bridge.ToString()|| SceneManager.GetActiveScene().name == ScenceName.PreparatoryPhase.ToString())
        //{
        //    return _panSpeed * Time.deltaTime * Mathf.Pow(transform.position.y, _heightSpeedScaling);
        //}
        //else
        //{
            return _panSpeed * Time.deltaTime / Time.timeScale* Mathf.Pow(transform.position.y, _heightSpeedScaling);
        //}
        
    }

    private float GetScaledZoomSpeed()
    {
        //if (SceneManager.GetActiveScene().name == ScenceName.Bridge.ToString() || SceneManager.GetActiveScene().name == ScenceName.PreparatoryPhase.ToString())
        //{
        //    return _zoomSpeed * Time.deltaTime * Mathf.Pow(transform.position.y, _heightSpeedScaling);
        //}
        //else
        //{
            return _zoomSpeed * Time.deltaTime / Time.timeScale * Mathf.Pow(transform.position.y, _heightSpeedScaling);
        //}
        
    }
   
    private void LateUpdate()
    {
        if (!CheckMouseInScreen()) return;
        var dx = _translateX < GetScaledPanSpeed() ? _translateX : GetScaledPanSpeed();
        var dz = _translateZ < GetScaledPanSpeed() ? _translateZ : GetScaledPanSpeed();
        _targetPosition += transform.TransformDirection(dx * Vector3.right);

        // If we move forward in local space, camera will also change altitude.
        // To properly move forward, we have to rotate the forward vector to be
        // horizontal in world space while keeping the magnitude:
        var worldForward = transform.TransformDirection(Vector3.forward);
        var angle = Quaternion.FromToRotation(worldForward, new Vector3(worldForward.x, 0, worldForward.z));
        _targetPosition += angle * worldForward * dz;

        _translateX -= dx;
        _translateZ -= dz;

        // Apply zoom movement:
        var dzoom = _leftoverZoom < GetScaledZoomSpeed() ? _leftoverZoom : GetScaledZoomSpeed();
        var oldAltitude = _targetPosition.y;

        // Zoom in:
        if (dzoom > 0) {
            ApplyZoomIn(dzoom);
        } else if (dzoom < 0) {
            ApplyZoomOut(dzoom);
        }

        _leftoverZoom -= dzoom;
        TiltCameraIfNearGround(oldAltitude);
        ClampCameraAltitude();
        ClampCameraXZPosition();

        // Note: It is mathematically incorrect to directly lerp on deltaTime like this,
        // since we would get infinitely closer to the target but never reach it.
        // However, it works without issues (because of rounding I guess):
        transform.position =
                //Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _panLerpSpeed);
                _targetPosition;
        transform.rotation =
                Quaternion.Slerp(
                        transform.rotation,
                        Quaternion.Euler(_rotateX, _rotateY, 0f),
                        Time.deltaTime * _rotLerpSpeed);

        // MaybeChangeTerrainMaterial();
    }

    /// <summary>
    /// When zooming in we gradually approach whatever the cursor is pointing at.
    /// </summary>
    private void AimedZoom()
    {
        var scroll = Mouse.current.scroll.ReadValue().y/1200;
        if (scroll == 0)
            return;
        // Zoom toward cursor:
        if (scroll > 0)
        {
            // Use a ray from the cursor to find what we'll be zooming into:
            Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            // If the cursor is not pointing at anything, zooming in is forbidden:
            if (!Physics.Raycast(ray, out hit))
            {
                return;
            }
            _zoomDestination = hit.point;
        }
        //if (_leftoverZoom + scroll * GetScaledZoomSpeed() < _minAltitude) return;
        //if (_leftoverZoom + scroll * GetScaledZoomSpeed() > _maxAltitude) return;
        _leftoverZoom += scroll * GetScaledZoomSpeed();
        Debug.Log(_leftoverZoom);
    }

    private void RotateCamera()
    {
        //if (SceneManager.GetActiveScene().name == ScenceName.Bridge.ToString()||SceneManager.GetActiveScene().name == ScenceName.PreparatoryPhase.ToString())
        //{
        //    _rotateX += -Input.GetAxis("Mouse Y") * _verticalRotationSpeed * Time.deltaTime ;
        //    _rotateY += Input.GetAxis("Mouse X") * _horizontalRotationSpeed * Time.deltaTime;
        //}
        //else
        //{
            _rotateX += -Mouse.current.delta.ReadValue().y* _verticalRotationSpeed * Time.deltaTime / Time.timeScale;
            _rotateY += Mouse.current.delta.ReadValue().x * _horizontalRotationSpeed * Time.deltaTime / Time.timeScale;
        //}
      
        // So we don't look too far up or down:
        _rotateX = Mathf.Clamp(_rotateX, _minCameraAngle, _maxCameraAngle);
    }

    private void ApplyZoomIn(float dzoom)
    {
        _targetPosition = Vector3.MoveTowards(_targetPosition, _zoomDestination, dzoom);
    }

    private void ApplyZoomOut(float dzoom)
    {
        Quaternion rotateToCamFacing = Quaternion.AngleAxis(_rotateY, Vector3.up);
        _targetPosition -= rotateToCamFacing * _zoomOutDirection * dzoom;
    }

    /// <summary>
    /// Camera looks down when high and up when low:
    /// </summary>
    /// <param name="oldAltitude"></param>
    private void TiltCameraIfNearGround(float oldAltitude)
    {
        if (transform.position.y < _tiltThreshold || _targetPosition.y < _tiltThreshold) {

            _rotateX += (_targetPosition.y - oldAltitude) * _zoomTiltSpeed;
            _rotateX = Mathf.Clamp(_rotateX, _minCameraAngle, _maxCameraAngle);
        }
    }

    private void ClampCameraAltitude()
    {
        //_targetPosition.y = Mathf.Clamp(
        //        _targetPosition.y,
        //        _terrain.SampleHeight(_targetPosition) + _minAltitude,
        //        _maxAltitude);
    }

    private void PanFromScreenBorder()
    {
        ScreenCorner mouseScreenCorner = GetScreenCornerForMousePosition(Mouse.current.position.ReadValue());

        //SetPanningCursor(mouseScreenCorner);

        switch (mouseScreenCorner) {

        case ScreenCorner.BottomLeft:
            _translateX += -1 * GetScaledPanSpeed();
            _translateZ += -1 * GetScaledPanSpeed();
            break;

        case ScreenCorner.BottomRight:
            _translateX += 1 * GetScaledPanSpeed();
            _translateZ += -1 * GetScaledPanSpeed();
            break;

        case ScreenCorner.TopLeft:
            _translateX += -1 * GetScaledPanSpeed();
            _translateZ += 1 * GetScaledPanSpeed();
            break;

        case ScreenCorner.TopRight:
            _translateX += 1 * GetScaledPanSpeed();
            _translateZ += 1 * GetScaledPanSpeed();
            break;

        case ScreenCorner.Left:
            _translateX += -1 * GetScaledPanSpeed();
            break;

        case ScreenCorner.Right:
            _translateX += 1 * GetScaledPanSpeed();
            break;

        case ScreenCorner.Bottom:
            _translateZ += -1 * GetScaledPanSpeed();
            break;

        case ScreenCorner.Top:
            _translateZ += 1 * GetScaledPanSpeed();
            break;

        case ScreenCorner.None:
            break;
        }
    }

    private ScreenCorner GetScreenCornerForMousePosition(Vector2 mousePosition)
    {
        if ((mousePosition.x <= _borderPanningOffset && mousePosition.x >= 0
                && mousePosition.y <= _borderPanningCornerSize
                && mousePosition.y >= 0)
                || (mousePosition.x <= _borderPanningCornerSize
                && mousePosition.x >= 0
                && mousePosition.y <= _borderPanningOffset && mousePosition.y >= 0)) {
            return ScreenCorner.BottomLeft;

        } else if ((mousePosition.x >= Screen.width - _borderPanningOffset
                && mousePosition.x <= Screen.width
                && mousePosition.y <= _borderPanningCornerSize && mousePosition.y >= 0)
                || (mousePosition.x >= Screen.width - _borderPanningCornerSize
                && mousePosition.x <= Screen.width
                && mousePosition.y <= _borderPanningOffset && mousePosition.y >= 0)) {
            return ScreenCorner.BottomRight;

        } else if ((mousePosition.x <= _borderPanningOffset
                && mousePosition.x >= 0
                && mousePosition.y >= Screen.height - _borderPanningCornerSize
                && mousePosition.y <= Screen.height)
                || (mousePosition.x <= _borderPanningCornerSize
                && mousePosition.x >= 0
                && mousePosition.y >= Screen.height - _borderPanningOffset
                && mousePosition.y <= Screen.height)) {
            return ScreenCorner.TopLeft;

        } else if ((mousePosition.x >= Screen.width - _borderPanningOffset
                && mousePosition.x <= Screen.width
                && mousePosition.y >= Screen.height - _borderPanningCornerSize
                && mousePosition.y <= Screen.height)
                || (mousePosition.x >= Screen.width - _borderPanningCornerSize
                && mousePosition.x <= Screen.width
                && mousePosition.y >= Screen.height - _borderPanningOffset
                && mousePosition.y <= Screen.height)) {
            return ScreenCorner.TopRight;

        } else if (mousePosition.x <= _borderPanningOffset
                && mousePosition.x >= 0
                && mousePosition.y >= 0
                && mousePosition.y <= Screen.height) {
            return ScreenCorner.Left;

        } else if (mousePosition.x >= Screen.width - _borderPanningOffset
                && mousePosition.x <= Screen.width
                && mousePosition.y >= 0 && mousePosition.y <= Screen.height) {
            return ScreenCorner.Right;

        } else if (mousePosition.y <= _borderPanningOffset
                && mousePosition.y >= 0
                && mousePosition.x >= 0
                && mousePosition.x <= Screen.width) {
            return ScreenCorner.Bottom;

        } else if (mousePosition.y >= Screen.height - _borderPanningOffset
                && mousePosition.y <= Screen.height
                && mousePosition.x >= 0
                && mousePosition.x <= Screen.width) {
            return ScreenCorner.Top;

        } else {
            return ScreenCorner.None;
        }
    }

    private void ClampCameraXZPosition()
    {
        //_targetPosition.x = Mathf.Clamp(
        //        _targetPosition.x,
        //        _terrain.GetPosition().x - _maxCameraHorizontalDistanceFromTerrain,
        //        _terrain.GetPosition().x + _terrain.terrainData.size.x + _maxCameraHorizontalDistanceFromTerrain);
        //_targetPosition.z = Mathf.Clamp(
        //        _targetPosition.z,
        //        _terrain.GetPosition().z - _maxCameraHorizontalDistanceFromTerrain,
        //        _terrain.GetPosition().z + _terrain.terrainData.size.z + _maxCameraHorizontalDistanceFromTerrain);
    }
    private bool CheckMouseInScreen()
    {
        float mouseX = Mouse.current.position.ReadValue().x;
        float mouseY = Mouse.current.position.ReadValue().y;
        if (Math.Abs(mouseX) <= 0 || Math.Abs(mouseY) <= 0) return false;
        if(Math.Abs(mouseX) > Math.Abs(Screen.width)|| Math.Abs(mouseY) > Math.Abs(Screen.height)) return false;
        return true;
    }
    private void SetPanningCursor(ScreenCorner corner)
    {
        Cursor.visible = false;
      //isableAllPanningArrows();
        switch (corner) {
        case ScreenCorner.TopLeft:
            _cornerArrowTopLeft.transform.position = Mouse.current.position.ReadValue();
            _cornerArrowTopLeft.enabled = true;
            break;
        case ScreenCorner.TopRight:
            _cornerArrowTopRight.transform.position = Mouse.current.position.ReadValue();
            _cornerArrowTopRight.enabled = true;
            break;
        case ScreenCorner.BottomLeft:
            _cornerArrowBottomLeft.transform.position = Mouse.current.position.ReadValue();
            _cornerArrowBottomLeft.enabled = true;
            break;
        case ScreenCorner.BottomRight:
            _cornerArrowBottomRight.transform.position = Mouse.current.position.ReadValue();
            _cornerArrowBottomRight.enabled = true;
            break;
        case ScreenCorner.Top:
            _sideArrowTop.transform.position = Mouse.current.position.ReadValue();
            _sideArrowTop.enabled = true;
            break;
        case ScreenCorner.Bottom:
            _sideArrowBottom.transform.position = Mouse.current.position.ReadValue();
            _sideArrowBottom.enabled = true;
            break;
        case ScreenCorner.Left:
            _sideArrowLeft.transform.position = Mouse.current.position.ReadValue();
            _sideArrowLeft.enabled = true;
            break;
        case ScreenCorner.Right:
            _sideArrowRight.transform.position = Mouse.current.position.ReadValue();
            _sideArrowRight.enabled = true;
            break;
        case ScreenCorner.None:
            Cursor.visible = true;
            break;
        default:
            break;
        }
    }

    //private void DisableAllPanningArrows()
    //{
    //    _cornerArrowBottomLeft.enabled = false;
    //    _cornerArrowBottomRight.enabled = false;
    //    _cornerArrowTopLeft.enabled = false;
    //    _cornerArrowTopRight.enabled = false;
    //    _sideArrowLeft.enabled = false;
    //    _sideArrowRight.enabled = false;
    //    _sideArrowTop.enabled = false;
    //    _sideArrowBottom.enabled = false;
    //}

    /// <summary>
    /// Based on camera distance, change terrain settings to improve appearance.
    ///
    /// TODO: we need to write our own shader instead of employing this hack
    /// </summary>
    //private void MaybeChangeTerrainMaterial()
    //{
    //    float camAltitude = transform.position.y - _terrain.transform.position.y;

    //    foreach (TerrainMaterial mat in _terrainMaterials) {
    //        if (camAltitude < mat.MaxAltitude) {

    //            if (_microSplatTerrain.templateMaterial != mat.Material) {

    //                _microSplatTerrain.templateMaterial = mat.Material;

    //                // In the inspector this is the "Debug/Keywords" field.
    //                _microSplatTerrain.keywordSO = mat.Keywords;
    //                // In the inspector this is the "Debug/Per Texture Data" field.
    //                _microSplatTerrain.propData = mat.PerTextureData;

    //                _microSplatTerrain.Sync();
    //            }
    //            break;
    //        }
    //    }
    //}

    enum ScreenCorner
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Top,
        Bottom,
        Left,
        Right,
        None
    }
}
