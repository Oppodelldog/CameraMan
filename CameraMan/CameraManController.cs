using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace CameraMan
{
    public class CameraManController : MonoBehaviour
    {
        [FormerlySerializedAs("track")] public CameraManTrackController trackController;

        private Vector3 _rot = Vector3.zero;
        private int _jumpIndex = -1;
        private Transform _attachedMarker;

        private void Awake()
        {
            Jotunn.Logger.LogInfo("CameraManController Awake");
        }

        private void Update()
        {
            if (trackController.IsPlaying) return;
            if (!ZInput.IsGamepadActive()) return;

            ApplyGeneralInput();
            ApplyPathWalkInput();
            ApplyRotation();
            ApplyTranslation();
            CarryAttachedMarker();
        }

        private void CarryAttachedMarker()
        {
            if (!IsAttachedToMarker()) return;
            _attachedMarker.position = transform.position;
            _attachedMarker.rotation = transform.rotation;
        }

        private void ApplyPathWalkInput()
        {
            if (ZInput.GetButtonDown("JoySit")) JumpPrevMarker();
            if (ZInput.GetButtonDown("JoyHotbarUse")) JumpNextMarker();
            if (ZInput.GetButtonDown("JoyDodge")) StopWalking();
        }

        private void StopWalking()
        {
            _jumpIndex = -1;
            _attachedMarker = null;
        }

        private void JumpPrevMarker()
        {
            _jumpIndex--;
            if (_jumpIndex < 0) _jumpIndex = trackController.GetMarkerCount() - 1;
            JumpToCurrentMarker();
        }

        private void JumpNextMarker()
        {
            _jumpIndex++;
            if (_jumpIndex >= trackController.GetMarkerCount()) _jumpIndex = 0;
            JumpToCurrentMarker();
        }

        private void JumpToCurrentMarker()
        {
            if (_jumpIndex < 0 || _jumpIndex >= trackController.GetMarkerCount()) return;
            _attachedMarker = trackController.GetMarker(_jumpIndex);
            transform.position = _attachedMarker.position;
            _rot = _attachedMarker.rotation.eulerAngles;
        }

        private bool IsAttachedToMarker()
        {
            return _jumpIndex != -1;
        }

        private void ApplyGeneralInput()
        {
            if (ZInput.GetButtonDown("JoyUse"))
            {
                trackController.AddMarker(transform);
            }

            if (ZInput.GetButtonDown("JoyJump") && trackController.CanTogglePlay())
            {
                StopWalking();
                trackController.Play();
            }

            if (ZInput.GetButtonDown("JoyCrouch"))
            {
                trackController.Clear();
            }
        }

        private void ApplyTranslation()
        {
            var move = Vector3.zero;
            move.x = ZInput.GetJoyLeftStickX() * 110f * Time.deltaTime * PlayerController.m_gamepadSens;
            move.z = ZInput.GetJoyLeftStickY() * -1 * 110.0f * Time.deltaTime * PlayerController.m_gamepadSens;
            move.y = ZInput.GetJoyRTrigger() - ZInput.GetJoyLTrigger() * 110.0f * Time.deltaTime * PlayerController.m_gamepadSens;

            var forward = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;
            var moveDirection = forward * move.z + transform.right * move.x + Vector3.up * move.y;
            transform.position += moveDirection * (Time.deltaTime * 5f);
        }

        private void ApplyRotation()
        {
            var mouseLook = Vector3.zero;

            if (!ZInput.GetButton("JoyRotate"))
            {
                mouseLook.x += ZInput.GetJoyRightStickX() * 110f * Time.deltaTime * PlayerController.m_gamepadSens;
                mouseLook.y += (float)(-(double)ZInput.GetJoyRightStickY() * 110.0) * Time.deltaTime * PlayerController.m_gamepadSens;

                var slope = ZInput.GetButton("JoyTabLeft") ? 1 : ZInput.GetButton("JoyTabRight") ? -1 : 0;
                mouseLook.z += slope * 10f * Time.deltaTime * PlayerController.m_gamepadSens;
            }

            if (PlayerController.m_invertCameraX)
                mouseLook.x *= -1f;
            if (PlayerController.m_invertCameraY)
                mouseLook.y *= -1f;

            _rot.y += mouseLook.x;
            _rot.x = Mathf.Clamp(_rot.x - mouseLook.y, -89f, 89f);
            _rot.z += mouseLook.z;

            transform.eulerAngles = _rot;
        }
    }
}