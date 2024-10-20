using System;
using BepInEx;
using Jotunn;
using UnityEngine;

namespace CameraMan
{
    [BepInDependency(Main.ModGuid)]
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    internal class Plugin : BaseUnityPlugin
    {
        // ReSharper disable block MemberCanBePrivate.Global
        public const string PluginGuid = "oppodelldog.mod.cameraman";
        public const string PluginName = "CameraMan";
        public const string PluginVersion = "0.1.0";

        private CameraManController _camController;
        private CameraManTrackController _trackControllerController;
        private CameraManUI _cameraManUI;
        private GameObject _hudRoot;
        private float _originalNearClipPlane;
        private PlayerController _playerController;
        private Camera _cam;
        private bool _isInitialized;

        private void Awake()
        {
            Jotunn.Logger.LogInfo("Awake");

            CameraMan.Config.Init(this);
        }

        private void OnDestroy()
        {
            CameraMan.Config.DoDisableConfigFileWatcher();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && _isInitialized) Enable(false);
            if (!Input.GetKeyDown(KeyCode.F10)) return;

            _cam = Camera.main;
            if (_cam == null)
            {
                Jotunn.Logger.LogInfo("CameraMan only works in game");
                return;
            }

            if (_hudRoot == null) _hudRoot = GameObject.Find("hudroot");
            if (_cameraManUI == null)
            {
                var uiObject = new GameObject("CameraManUI");
                uiObject.transform.SetParent(null);
                _cameraManUI = uiObject.AddComponent<CameraManUI>();
            }

            if (_trackControllerController == null)
            {
                var trackControllerObject = new GameObject("CameraManTrack");
                trackControllerObject.transform.SetParent(null);
                _trackControllerController = trackControllerObject.AddComponent<CameraManTrackController>();
                _trackControllerController.ui = _cameraManUI;
            }

            if (_camController == null)
            {
                _camController = _cam.gameObject.AddComponent<CameraManController>();
                _camController.trackController = _trackControllerController;
                _trackControllerController.cam = _cam.transform;
            }

            var player = Player.m_localPlayer;
            _playerController = player.GetComponent<PlayerController>();
            var vanillaEnabled = !_playerController.enabled;
            Enable(!vanillaEnabled);
            _isInitialized = true;
        }

        private void Enable(bool value)
        {
            _playerController.enabled = !value;
            _cam.GetComponent<GameCamera>().enabled = !value;
            _hudRoot.SetActive(!value);

            if (value)
            {
                _originalNearClipPlane = _cam.nearClipPlane;
                _cam.nearClipPlane = 0.01f;
            }
            else
            {
                _cam.nearClipPlane = _originalNearClipPlane;
            }

            _camController.enabled = value;
            _trackControllerController.enabled = value;
            _cameraManUI.ShowUI(value);
        }
    }
}