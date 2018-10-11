﻿// Copyright Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace GalaxyExplorer
{
    /// <summary>
    /// Class for manually controlling the camera when HuP isn't running. Attach to same CameraRig object as the StereoCamera.cs script.
    /// </summary>
    public class ManualCameraControl : MonoBehaviour
    {
        public enum MouseButton
        {
            Left,
            Right,
            Middle,
            Control,
            Shift,
            Focused,
            None
        }

        public float ExtraMouseSensitivityScale = 3.0f;
        public float DefaultMouseSensitivity = 0.1f;
        public MouseButton MouseLookButton = MouseButton.Control;
        public bool IsControllerLookInverted = true;
        private bool isMouseJumping = false;

        private bool isGamepadLookEnabled = true;
        private bool isFlyKeypressEnabled = true;
        private Vector3 lastMousePosition = Vector3.zero;
        private Vector3 lastTrackerToUnityTranslation = Vector3.zero;
        private Quaternion lastTrackerToUnityRotation = Quaternion.identity;
        private bool appHasFocus = true;
        private bool wasLooking = false;

        private static float InputCurve(float x)
        {
            // smoothing input curve, converts from [-1,1] to [-2,2]
            return Mathf.Sign(x) * (1.0f - Mathf.Cos(0.5f * Mathf.PI * Mathf.Clamp(x, -1.0f, 1.0f)));
        }

#if UNITY_EDITOR
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern uint GetCurrentProcessId();

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool ProcessIdToSessionId(uint dwProcessId, out uint pSessionId);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern uint WTSGetActiveConsoleSessionId();
#endif

        // While running in the Editor allow to the mouse to simulate HoloLens head tracking.
        // While running outside of the Editor treat the mouse as operating over a 2D view of the 
        // 3D GalaxyExplorer world space
#if UNITY_EDITOR
        private void Awake()
        {
            // Workaround for Remote Desktop.  Without this, Game window mousing breaks in modes
            // that use Unity's Cursor.lockState feature.
            if (IsRunningUnderRemoteDesktop())
            {
                if (this.MouseLookButton >= MouseButton.Control && this.MouseLookButton <= MouseButton.Focused)
                {
                    this.MouseLookButton = MouseButton.None;
                    Debug.Log("Running under Remote Desktop, so changed MouseLook method to None");
                }
            }
        }

        private void Update()
        {
            // Undo the last tracker to Unity transforms applied
            this.transform.Translate(-this.lastTrackerToUnityTranslation, Space.World);
            this.transform.Rotate(-this.lastTrackerToUnityRotation.eulerAngles, Space.World);

            // Calculate and apply the camera control movement this frame
            Vector3 rotate = GetCameraControlRotation();
            Vector3 translate = GetCameraControlTranslation();

            this.transform.Rotate(rotate.x, 0.0f, 0.0f);
            this.transform.Rotate(0.0f, rotate.y, 0.0f, Space.World);
            this.transform.Translate(translate, Space.World);

            this.transform.Rotate(this.lastTrackerToUnityRotation.eulerAngles, Space.World);
            this.transform.Translate(this.lastTrackerToUnityTranslation, Space.World);
        }
#endif

        private float GetKeyDir(string neg, string pos)
        {
            return Input.GetKey(neg) ? -1.0f : Input.GetKey(pos) ? 1.0f : 0.0f;
        }

        private Vector3 GetCameraControlTranslation()
        {
            Vector3 deltaPosition = Vector3.zero;
            deltaPosition += GetKeyDir("left", "right") * this.transform.right;
            deltaPosition += GetKeyDir("down", "up") * this.transform.forward;

            // Support fly up/down keypresses if the current project maps it. This isn't a standard
            // Unity InputManager mapping, so it has to gracefully fail if unavailable.
            if (this.isFlyKeypressEnabled)
            {
                try
                {
                    deltaPosition += InputCurve(Input.GetAxis("Fly")) * this.transform.up;
                }
                catch (System.Exception)
                {
                    this.isFlyKeypressEnabled = false;
                }
            }

            deltaPosition += InputCurve(Input.GetAxis("Horizontal")) * this.transform.right;
            deltaPosition += InputCurve(Input.GetAxis("Vertical")) * this.transform.forward;

            float accel = Input.GetKey(KeyCode.LeftShift) ? 1.0f : 0.1f;
            return accel * deltaPosition;
        }

        private Vector3 GetCameraControlRotation()
        {
            Vector3 rot = Vector3.zero;

            float inversionFactor = this.IsControllerLookInverted ? -1.0f : 1.0f;

            if (this.isGamepadLookEnabled)
            {
                try
                {
                    // Get the axes information from the right stick of X360 controller
                    rot.x += InputCurve(Input.GetAxis("LookVertical")) * inversionFactor;
                    rot.y += InputCurve(Input.GetAxis("LookHorizontal"));
                }
                catch (System.Exception)
                {
                    this.isGamepadLookEnabled = false;
                }
            }

            if (this.ShouldMouseLook)
            {
                if (!this.wasLooking)
                {
                    OnStartMouseLook();
                }

                ManualCameraControl_MouseLookTick(ref rot);

                this.wasLooking = true;
            }
            else
            {
                if (this.wasLooking)
                {
                    OnEndMouseLook();
                }

                this.wasLooking = false;
            }

            rot *= this.ExtraMouseSensitivityScale;

            return rot;
        }

        private void OnStartMouseLook()
        {
            if (this.MouseLookButton <= MouseButton.Middle)
            {
                // if mousebutton is either left, right or middle
                SetWantsMouseJumping(true);
            }
            else if (this.MouseLookButton <= MouseButton.Focused)
            {
                // if mousebutton is either control, shift or focused
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;
            }

            // do nothing if (this.MouseLookButton == MouseButton.None)
        }

        private void OnEndMouseLook()
        {
            if (this.MouseLookButton <= MouseButton.Middle)
            {
                // if mousebutton is either left, right or middle
                SetWantsMouseJumping(false);
            }
            else if (this.MouseLookButton <= MouseButton.Focused)
            {
                // if mousebutton is either control, shift or focused
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
            }

            // do nothing if (this.MouseLookButton == MouseButton.None)
        }

        private void ManualCameraControl_MouseLookTick(ref Vector3 rot)
        {
            // Use frame-to-frame mouse delta in pixels to determine mouse rotation. The traditional
            // GetAxis("Mouse X") method doesn't work under Remote Desktop.
            Vector3 mousePositionDelta = Input.mousePosition - this.lastMousePosition;
            this.lastMousePosition = Input.mousePosition;

            if (UnityEngine.Cursor.lockState == CursorLockMode.Locked)
            {
                mousePositionDelta.x = Input.GetAxis("Mouse X");
                mousePositionDelta.y = Input.GetAxis("Mouse Y");
            }
            else
            {
                mousePositionDelta.x *= this.DefaultMouseSensitivity;
                mousePositionDelta.y *= this.DefaultMouseSensitivity;
            }

            rot.x += -InputCurve(mousePositionDelta.y);
            rot.y += InputCurve(mousePositionDelta.x);
        }

        private bool ShouldMouseLook
        {
            get
            {
                // Only allow the mouse to control rotation when Unity has focus. This enables
                // the player to temporarily alt-tab away without having the player look around randomly
                // back in the Unity Game window.
                if (!this.appHasFocus)
                {
                    return false;
                }
                else if (this.MouseLookButton == MouseButton.None)
                {
                    return true;
                }
                else if (this.MouseLookButton <= MouseButton.Middle)
                {
                    return Input.GetMouseButton((int)this.MouseLookButton);
                }
                else if (this.MouseLookButton == MouseButton.Control)
                {
                    return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                }
                else if (this.MouseLookButton == MouseButton.Shift)
                {
                    return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                }
                else if (this.MouseLookButton == MouseButton.Focused)
                {
                    if (!this.wasLooking)
                    {
                        // any kind of click will capture focus
                        return Input.GetMouseButtonDown((int)MouseButton.Left) || Input.GetMouseButtonDown((int)MouseButton.Right) || Input.GetMouseButtonDown((int)MouseButton.Middle);
                    }
                    else
                    {
                        // pressing escape will stop capture
                        return !Input.GetKeyDown(KeyCode.Escape);
                    }
                }

                return false;
            }
        }

        private void OnApplicationFocus(bool focusStatus)
        {
            this.appHasFocus = focusStatus;
        }

        private void SetWantsMouseJumping(bool wantsJumping)
        {
            if (wantsJumping != this.isMouseJumping)
            {
                this.isMouseJumping = wantsJumping;

                if (wantsJumping)
                {
                    // unlock the cursor if it was locked
                    UnityEngine.Cursor.lockState = CursorLockMode.None;

                    // hide the cursor
                    UnityEngine.Cursor.visible = false;

                    this.lastMousePosition = Input.mousePosition;
                }
                else
                {
                    // recenter the cursor (setting lockCursor has side-effects under the hood)
                    UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                    UnityEngine.Cursor.lockState = CursorLockMode.None;

                    // show the cursor
                    UnityEngine.Cursor.visible = true;
                }

#if UNITY_EDITOR
                UnityEditor.EditorGUIUtility.SetWantsMouseJumping(wantsJumping ? 1 : 0);
#endif
            }
        }

#if UNITY_EDITOR
        private bool IsRunningUnderRemoteDesktop()
        {
            uint processId = GetCurrentProcessId();
            uint sessionId;
            return ProcessIdToSessionId(processId, out sessionId) && (sessionId != WTSGetActiveConsoleSessionId());
        }
#else
        private bool IsRunningUnderRemoteDesktop()
        {
            return false;
        }
#endif
    }
}