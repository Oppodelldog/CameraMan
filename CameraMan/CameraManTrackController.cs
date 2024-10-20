using System;
using UnityEngine;

namespace CameraMan
{
    public class CameraManTrackController : MonoBehaviour
    {
        public bool IsPlaying { get; private set; }
        public Transform cam;
        public CameraManUI ui;

        private Transform _placedMarkers;
        private Transform _smoothedMarkers;
        private Transform _playbackContainer;
        private int _trackIndex;
        private Vector3 _originalCamPos = Vector3.zero;
        private Quaternion _originalCamRot = Quaternion.identity;
        private float _playToggledSince;

        private void OnEnable()
        {
            _placedMarkers.gameObject.SetActive(true);
            _smoothedMarkers.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            _placedMarkers.gameObject.SetActive(false);
            _smoothedMarkers.gameObject.SetActive(false);
            StopPlayback();
        }

        private void Awake()
        {
            Jotunn.Logger.LogInfo("CameraManTrackController Awake");
            _placedMarkers = new GameObject("placed_markers").transform;
            _placedMarkers.SetParent(null);

            _smoothedMarkers = new GameObject("smoothed_markers").transform;
            _smoothedMarkers.SetParent(null);

            _playbackContainer = _smoothedMarkers;

            Config.Visibility.ShowTrackMarkers.SettingChanged += OnConfigUpdateShowTrackMarkers;
            Config.Visibility.ShowPlaybackTrackMarkers.SettingChanged += OnConfigUpdateShowPlaybackTrackMarkers;
            Config.Visibility.ShowTrackMarkersDuringPlayback.SettingChanged += OnConfigUpdateShowTrackMarkersDuringPlayback;
            Config.Visibility.ShowPlaybackTrackMarkersDuringPlayback.SettingChanged += OnConfigUpdateShowPlaybackTrackMarkersDuringPlayback;
            Config.Settings.TrackResolution.SettingChanged += OnConfigUpdateTrackResolution;
            Config.Settings.SmoothingSteps.SettingChanged += OnConfigUpdateSmoothingSteps;
        }

        private void OnConfigUpdateSmoothingSteps(object sender, EventArgs e)
        {
            if (IsPlaying) return;
            GenerateSmoothTrack();
        }

        private void OnConfigUpdateTrackResolution(object sender, EventArgs e)
        {
            if (IsPlaying) return;
            GenerateSmoothTrack();
        }

        private void OnConfigUpdateShowPlaybackTrackMarkersDuringPlayback(object sender, EventArgs e)
        {
            if (!IsPlaying) return;
            EnableRenderer(_smoothedMarkers, Config.Visibility.ShowPlaybackTrackMarkersDuringPlayback.Value);
        }

        private void OnConfigUpdateShowTrackMarkersDuringPlayback(object sender, EventArgs e)
        {
            if (!IsPlaying) return;
            EnableRenderer(_placedMarkers, Config.Visibility.ShowTrackMarkersDuringPlayback.Value);
        }

        private void OnConfigUpdateShowTrackMarkers(object sender, EventArgs e)
        {
            if (IsPlaying) return;
            EnableRenderer(_placedMarkers, Config.Visibility.ShowTrackMarkers.Value);
        }

        private void OnConfigUpdateShowPlaybackTrackMarkers(object sender, EventArgs e)
        {
            if (IsPlaying) return;
            EnableRenderer(_smoothedMarkers, Config.Visibility.ShowPlaybackTrackMarkers.Value);
        }

        public bool CanTogglePlay()
        {
            return _playToggledSince + 0.5f < Time.time;
        }

        private void Update()
        {
            if (!IsPlaying) return;
            if (_playbackContainer.childCount == 0)
            {
                Jotunn.Logger.LogWarning("No track to play, stopping playback");
                FinishPlayback();
                return;
            }

            if (ZInput.GetButtonDown("JoyJump") && CanTogglePlay())
            {
                FinishPlayback();
                return;
            }

            var nextPos = _playbackContainer.GetChild(_trackIndex).position;
            var nextRot = _playbackContainer.GetChild(_trackIndex).rotation;

            var speed = Config.Settings.PlaybackSpeed.Value * Config.Settings.PlaybackSpeedMultiplier.Value;
            var distance = Vector3.Distance(cam.transform.position, nextPos);
            var rotationSpeed = Quaternion.Angle(cam.transform.rotation, nextRot) / distance * Time.deltaTime * speed;

            cam.transform.position = Vector3.MoveTowards(cam.transform.position, nextPos, Time.deltaTime * speed);
            cam.transform.rotation = Quaternion.RotateTowards(cam.transform.rotation, nextRot, rotationSpeed);


            if (!(Vector3.Distance(cam.transform.position, nextPos) < 0.1f)) return;
            _trackIndex++;
            Jotunn.Logger.LogInfo($"Track index {_trackIndex} of {_playbackContainer.childCount} pos {nextPos} rot {nextRot}");

            if (_trackIndex < _playbackContainer.childCount) return;

            Jotunn.Logger.LogWarning($"Playback finished at index {_trackIndex} of {_playbackContainer.childCount}");
            FinishPlayback();
        }

        public void Play()
        {
            if (IsPlaying)
            {
                Jotunn.Logger.LogInfo("Track playback already running");
                return;
            }

            StartTrackPlayback();
        }

        private void GenerateSmoothTrack()
        {
            Jotunn.Logger.LogInfo("Clean up old track");
            CleanupSmoothTrack();

            if (_placedMarkers.childCount < 2)
            {
                Jotunn.Logger.LogInfo("Not enough markers to create track");
                return;
            }

            Jotunn.Logger.LogInfo("Prepare smooth track, create high definition copy of track");
            for (var i = 0; i < _placedMarkers.childCount - 1; i++)
            {
                var pointA = _placedMarkers.GetChild(i);
                var pointB = _placedMarkers.GetChild(i + 1);

                Instantiate(pointA, _smoothedMarkers);

                var res = Config.Settings.TrackResolution.Value;
                for (var step = 1; step <= res; step++)
                {
                    var t = (float)step / (res + 1);

                    var interpolatedPosition = Vector3.Lerp(pointA.position, pointB.position, t);
                    var interpolatedRotation = Quaternion.Slerp(pointA.rotation, pointB.rotation, t);

                    var newPoint = Instantiate(pointA, _smoothedMarkers);
                    newPoint.transform.position = interpolatedPosition;
                    newPoint.transform.rotation = interpolatedRotation;
                }
            }

            Instantiate(_placedMarkers.GetChild(_placedMarkers.childCount - 1), _smoothedMarkers);

            var smoothSteps = Config.Settings.SmoothingSteps.Value;
            Smoothing.SmoothTrack(_smoothedMarkers, smoothSteps);

            EnableRenderer(_smoothedMarkers, Config.Visibility.ShowPlaybackTrackMarkers.Value);

            Jotunn.Logger.LogInfo($"Smooth high definition track created with {smoothSteps} steps");
        }

        private void CleanupSmoothTrack()
        {
            foreach (Transform child in _smoothedMarkers)
            {
                Destroy(child.gameObject);
            }

            _smoothedMarkers = new GameObject("smoothed_markers").transform;
            _smoothedMarkers.SetParent(null);
            _playbackContainer = _smoothedMarkers;
        }

        private void FinishPlayback()
        {
            Jotunn.Logger.LogInfo("Track playback finished");

            cam.transform.position = _originalCamPos;
            cam.transform.rotation = _originalCamRot;

            Jotunn.Logger.LogInfo("Camera reset");
            
            EnableRenderer(_placedMarkers, Config.Visibility.ShowTrackMarkers.Value);
            EnableRenderer(_smoothedMarkers, Config.Visibility.ShowPlaybackTrackMarkers.Value);

            ui.ShowUI(true);
            StopPlayback();
        }

        private void StopPlayback()
        {
            _trackIndex = 0;
            _playToggledSince = Time.time;
            IsPlaying = false;
        }

        private void StartTrackPlayback()
        {
            Jotunn.Logger.LogInfo("Start track playback");
            _originalCamPos = cam.transform.position;
            _originalCamRot = cam.transform.rotation;

            GenerateSmoothTrack();
            EnableRenderer(_placedMarkers, Config.Visibility.ShowTrackMarkersDuringPlayback.Value);
            EnableRenderer(_smoothedMarkers, Config.Visibility.ShowPlaybackTrackMarkersDuringPlayback.Value);

            ui.ShowUI(Config.Visibility.ShowPanelDuringPlayback.Value);

            IsPlaying = true;
            _trackIndex = 0;
            _playToggledSince = Time.time;
            cam.transform.position = _playbackContainer.GetChild(_trackIndex).position;
            cam.transform.rotation = _playbackContainer.GetChild(_trackIndex).rotation;
        }

        private static void EnableRenderer(Transform t, bool enable)
        {
            foreach (Transform child in t)
            {
                child.GetComponent<Renderer>().enabled = enable;
            }

            var shader = t.GetChild(0).GetComponent<Renderer>().material.shader;
            for (var i = 0; i < shader.GetPropertyCount(); i++)
            {
                var propertyName = shader.GetPropertyName(i);
                var propertyType = shader.GetPropertyType(i);
                Jotunn.Logger.LogInfo("Shader prop: " + propertyName + " type: " + propertyType);
            }
        }

        public void Clear()
        {
            Jotunn.Logger.LogInfo("Clear cam track");
            foreach (Transform child in _placedMarkers)
            {
                Destroy(child.gameObject);
            }

            CleanupSmoothTrack();

            Jotunn.Logger.LogInfo("Cleared cam track");

            if (IsPlaying)
            {
                Jotunn.Logger.LogInfo("Stop playback, track has been cleared");
                FinishPlayback();
            }
        }

        public void AddMarker(Transform t)
        {
            Jotunn.Logger.LogInfo("Add cam track marker");
            var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = "marker";
            marker.transform.position = t.position;
            marker.transform.rotation = t.rotation;
            marker.transform.localScale = Vector3.one * 0.1f;
            marker.transform.SetParent(_placedMarkers);
            var renderer = marker.GetComponent<Renderer>();
            renderer.material.shader = Shader.Find("ToonDeferredShading2017");
            renderer.material.color = new Color(1, 1, 1, 0.1f);
            renderer.enabled = Config.Visibility.ShowTrackMarkers.Value;

            Jotunn.Logger.LogInfo($"Added marker at {t.position} with rotation {t.rotation}");
        }

        public int GetMarkerCount()
        {
            return _placedMarkers.childCount;
        }

        public Transform GetMarker(int jumpIndex)
        {
            return _placedMarkers.GetChild(jumpIndex);
        }
    }
}