using System.Collections.Generic;
using System.Globalization;
using BepInEx.Configuration;
using Jotunn.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace CameraMan
{
    public class CameraManUI : MonoBehaviour
    {
        private GameObject _panelObj;
        private GameObject _speedMultiplierDropdown;

        private Dropdown _dropdown;
        private GameObject _speedMultiplierDropdownLabel;
        private GameObject _panelTitle;
        private GameObject _showTrackMarkersCheckbox;
        private GameObject _showTrackMarkersCheckboxLabel;
        private GameObject _showPlaybackTrackMarkersCheckbox;
        private GameObject _showPlaybackTrackMarkersCheckboxLabel;
        private GameObject _showTrackMarkersDuringPlaybackCheckbox;
        private GameObject _showTrackMarkersDuringPlaybackCheckboxLabel;
        private GameObject _showPlaybackTrackMarkersDuringPlaybackCheckbox;
        private GameObject _showPlaybackTrackMarkersDuringPlaybackCheckboxLabel;
        private GameObject _showPanelDuringPlaybackCheckbox;
        private GameObject _showPanelDuringPlaybackCheckboxLabel;
        private GameObject _trackResolutionInput;
        private GameObject _trackResolutionLabel;
        private GameObject _smoothingStepsInput;
        private GameObject _smoothingStepsLabel;

        private void Awake()
        {
            var titleFontSize = Config.UI.TitleFontSize.Value;
            var labelFontSize = Config.UI.LabelFontSize.Value;
            var dropDownFontSize = Config.UI.DropDownFontSize.Value;

            var topLeftAnchor = new Vector2(0f, 1f);
            var centerAnchor = new Vector2(0.5f, 0.5f);
            var topCenterAnchor = new Vector2(0.5f, 1);
            var bottomLeftAnchor = new Vector2(0f, 0f);
            var bottomRightAnchor = new Vector2(1f, 0f);

            var panelSizePos = new SizeAndPosition(Config.UI.PanelSizeAndPosition.Value);
            var panelTitleSizePos = new SizeAndPosition(Config.UI.PanelTitleSizeAndPosition.Value);
            var speedMultiplierSizePos = new SizeAndPosition(Config.UI.SpeedMultiplierSizeAndPosition.Value);
            var speedMultiplierLabelSizePos = new SizeAndPosition(Config.UI.SpeedMultiplierLabelSizeAndPosition.Value);
            
            _panelObj = GUIManager.Instance.CreateWoodpanel(
                parent: GUIManager.CustomGUIFront.transform,
                anchorMin: bottomRightAnchor,
                anchorMax: bottomRightAnchor,
                position: panelSizePos.Position,
                width: panelSizePos.Width,
                height: panelSizePos.Height,
                draggable: true);
            _panelObj.GetComponent<RectTransform>().pivot = bottomRightAnchor;

            _panelTitle = GUIManager.Instance.CreateText(
                text: "Camera - Man",
                parent: _panelObj.transform,
                anchorMin: topCenterAnchor,
                anchorMax: topCenterAnchor,
                position: panelTitleSizePos.Position,
                font: GUIManager.Instance.AveriaSerifBold,
                fontSize: titleFontSize,
                color: GUIManager.Instance.ValheimOrange,
                outline: true,
                outlineColor: Color.black,
                width: panelTitleSizePos.Width,
                height: panelTitleSizePos.Height,
                addContentSizeFitter: false);
            _panelTitle.GetComponent<RectTransform>().pivot = topCenterAnchor;

            _speedMultiplierDropdownLabel = GUIManager.Instance.CreateText(
                text: "Speed x",
                parent: _panelObj.transform,
                anchorMin: topLeftAnchor,
                anchorMax: topLeftAnchor,
                position: speedMultiplierLabelSizePos.Position,
                font: GUIManager.Instance.AveriaSerifBold,
                fontSize: labelFontSize,
                color: GUIManager.Instance.ValheimOrange,
                outline: true,
                outlineColor: Color.black,
                width: speedMultiplierLabelSizePos.Width,
                height: speedMultiplierLabelSizePos.Height,
                addContentSizeFitter: false);
            _speedMultiplierDropdownLabel.GetComponent<RectTransform>().pivot = topLeftAnchor;

            _speedMultiplierDropdown = GUIManager.Instance.CreateDropDown(
                parent: _panelObj.transform,
                anchorMin: topLeftAnchor,
                anchorMax: topLeftAnchor,
                position: speedMultiplierSizePos.Position,
                fontSize: dropDownFontSize,
                width: speedMultiplierSizePos.Width,
                height: speedMultiplierSizePos.Height);
            _speedMultiplierDropdown.GetComponent<RectTransform>().pivot = topLeftAnchor;
            _dropdown = _speedMultiplierDropdown.GetComponent<Dropdown>();
            _dropdown.AddOptions(new List<string>
            {
                "0.1", "1", "10",
            });
            _dropdown.value = _dropdown.options.FindIndex(option => option.text == Config.Settings.PlaybackSpeedMultiplier.Value.ToString(CultureInfo.InvariantCulture));
            _dropdown.onValueChanged.AddListener(OnSpeedMultiplierChanged);

            CreateInputField(topLeftAnchor, "Track Resolution",
                new SizeAndPosition(Config.UI.TrackResolutionSizeAndPosition.Value), new SizeAndPosition(Config.UI.TrackResolutionLabelSizeAndPosition.Value),
                Config.Settings.TrackResolution,
                out _trackResolutionInput, out _trackResolutionLabel);
            CreateInputField(topLeftAnchor, "Smoothing Steps",
                new SizeAndPosition(Config.UI.SmoothingStepsSizeAndPosition.Value), new SizeAndPosition(Config.UI.SmoothingStepsLabelSizeAndPosition.Value),
                Config.Settings.SmoothingSteps,
                out _smoothingStepsInput, out _smoothingStepsLabel);

            CreateCheckbox(topLeftAnchor, "Show Track Markers",
                new SizeAndPosition(Config.UI.ShowTrackMarkersSizeAndPosition.Value), new SizeAndPosition(Config.UI.ShowTrackMarkersLabelSizeAndPosition.Value),
                Config.Visibility.ShowTrackMarkers,
                out _showTrackMarkersCheckbox, out _showTrackMarkersCheckboxLabel);
            CreateCheckbox(topLeftAnchor, "Show Playback Track Markers",
                new SizeAndPosition(Config.UI.ShowPlaybackTrackMarkersSizeAndPosition.Value), new SizeAndPosition(Config.UI.ShowPlaybackTrackMarkersLabelSizeAndPosition.Value),
                Config.Visibility.ShowPlaybackTrackMarkers,
                out _showPlaybackTrackMarkersCheckbox, out _showPlaybackTrackMarkersCheckboxLabel);
            CreateCheckbox(topLeftAnchor, "Show Track Markers During Playback",
                new SizeAndPosition(Config.UI.ShowTrackMarkersDuringPlaybackSizeAndPosition.Value), new SizeAndPosition(Config.UI.ShowTrackMarkersDuringPlaybackLabelSizeAndPosition.Value),
                Config.Visibility.ShowTrackMarkersDuringPlayback,
                out _showTrackMarkersDuringPlaybackCheckbox, out _showTrackMarkersDuringPlaybackCheckboxLabel);
            CreateCheckbox(topLeftAnchor, "Show Playback Track Markers During Playback",
                new SizeAndPosition(Config.UI.ShowPlaybackTrackMarkersDuringPlaybackSizeAndPosition.Value), new SizeAndPosition(Config.UI.ShowPlaybackTrackMarkersDuringPlaybackLabelSizeAndPosition.Value),
                Config.Visibility.ShowPlaybackTrackMarkersDuringPlayback,
                out _showPlaybackTrackMarkersDuringPlaybackCheckbox, out _showPlaybackTrackMarkersDuringPlaybackCheckboxLabel);
            CreateCheckbox(topLeftAnchor, "Show Panel During Playback",
                new SizeAndPosition(Config.UI.ShowPanelDuringPlaybackSizeAndPosition.Value), new SizeAndPosition(Config.UI.ShowPanelDuringPlaybackLabelSizeAndPosition.Value),
                Config.Visibility.ShowPanelDuringPlayback,
                out _showPanelDuringPlaybackCheckbox, out _showPanelDuringPlaybackCheckboxLabel);

            InitConfigReload();
        }

        private void CreateInputField(Vector2 topLeftAnchor, string title, SizeAndPosition input, SizeAndPosition label, ConfigEntry<int> configEntry, out GameObject inputField, out GameObject labelObj)
        {
            inputField = GUIManager.Instance.CreateInputField(
                parent: _panelObj.transform,
                anchorMin: topLeftAnchor,
                anchorMax: topLeftAnchor,
                position: input.Position,
                contentType: InputField.ContentType.Standard,
                placeholderText: "",
                fontSize: Config.UI.InputFieldFontSize.Value,
                width: input.Width,
                height: input.Height);
            inputField.GetComponent<RectTransform>().pivot = topLeftAnchor;
            inputField.GetComponent<InputField>().text = configEntry.Value.ToString();
            inputField.GetComponent<InputField>().onValueChanged.AddListener((value) =>
            {
                if (int.TryParse(value, out var intValue))
                {
                    configEntry.Value = intValue;
                }
            });
            
            CreateLabel(topLeftAnchor, title, label, out labelObj);
        }

        private void CreateCheckbox(Vector2 anchor, string title,
            SizeAndPosition checkbox, SizeAndPosition label,
            ConfigEntry<bool> configEntry,
            out GameObject cb, out GameObject lbl)
        {
            cb = GUIManager.Instance.CreateToggle(parent: _panelObj.transform, width: 40f, height: 40f);
            cb.GetComponent<RectTransform>().anchorMin = anchor;
            cb.GetComponent<RectTransform>().anchorMax = anchor;
            cb.GetComponent<RectTransform>().pivot = anchor;
            cb.GetComponent<RectTransform>().anchoredPosition = checkbox.Position;
            CreateLabel(anchor, title, label, out lbl);

            var toggle = cb.GetComponent<Toggle>();
            toggle.isOn = configEntry.Value;
            toggle.onValueChanged.AddListener((value) => configEntry.Value = value);
        }

        private void CreateLabel(Vector2 anchor, string title, SizeAndPosition label, out GameObject lbl)
        {
            lbl = GUIManager.Instance.CreateText(
                text: title,
                parent: _panelObj.transform,
                anchorMin: anchor,
                anchorMax: anchor,
                position: label.Position,
                font: GUIManager.Instance.AveriaSerifBold,
                fontSize: Config.UI.LabelFontSize.Value,
                color: GUIManager.Instance.ValheimOrange,
                outline: true,
                outlineColor: Color.black,
                width: label.Width,
                height: label.Height,
                addContentSizeFitter: false);
            lbl.GetComponent<RectTransform>().pivot = anchor;
        }

        private void OnSpeedMultiplierChanged(int arg0)
        {
            var value = _dropdown.options[arg0].text;
            if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var speedMultiplier))
            {
                Jotunn.Logger.LogWarning($"Failed to parse speed multiplier value {value}");
                return;
            }

            Config.Settings.PlaybackSpeedMultiplier.Value = speedMultiplier;
            Jotunn.Logger.LogInfo($"Speed multiplier set to {speedMultiplier}");
        }

        private void InitConfigReload()
        {
            CameraManUIConfigChanges.ApplyConfigChangesForSizeAndPosition(_panelObj, Config.UI.PanelSizeAndPosition);
            CameraManUIConfigChanges.ApplyConfigChangesForSizeAndPosition(_panelTitle, Config.UI.PanelTitleSizeAndPosition);
            CameraManUIConfigChanges.ApplyConfigChangesForSizeAndPosition(_speedMultiplierDropdownLabel, Config.UI.SpeedMultiplierLabelSizeAndPosition);
            CameraManUIConfigChanges.ApplyConfigChangesForSizeAndPosition(_speedMultiplierDropdown, Config.UI.SpeedMultiplierSizeAndPosition);
            CameraManUIConfigChanges.ApplyConfigChangesForSizeAndPosition(_showTrackMarkersCheckbox, Config.UI.ShowTrackMarkersSizeAndPosition);
            CameraManUIConfigChanges.ApplyConfigChangesForSizeAndPosition(_showTrackMarkersCheckboxLabel, Config.UI.ShowTrackMarkersLabelSizeAndPosition);
            CameraManUIConfigChanges.ApplyConfigChangesForSizeAndPosition(_showPlaybackTrackMarkersCheckbox, Config.UI.ShowPlaybackTrackMarkersSizeAndPosition);
            CameraManUIConfigChanges.ApplyConfigChangesForSizeAndPosition(_showPlaybackTrackMarkersCheckboxLabel, Config.UI.ShowPlaybackTrackMarkersLabelSizeAndPosition);
            CameraManUIConfigChanges.ApplyConfigChangesForSizeAndPosition(_showTrackMarkersDuringPlaybackCheckbox, Config.UI.ShowTrackMarkersDuringPlaybackSizeAndPosition);
            CameraManUIConfigChanges.ApplyConfigChangesForSizeAndPosition(_showTrackMarkersDuringPlaybackCheckboxLabel, Config.UI.ShowTrackMarkersDuringPlaybackLabelSizeAndPosition);
            CameraManUIConfigChanges.ApplyConfigChangesForSizeAndPosition(_showPlaybackTrackMarkersDuringPlaybackCheckbox, Config.UI.ShowPlaybackTrackMarkersDuringPlaybackSizeAndPosition);
            CameraManUIConfigChanges.ApplyConfigChangesForSizeAndPosition(_showPlaybackTrackMarkersDuringPlaybackCheckboxLabel, Config.UI.ShowPlaybackTrackMarkersDuringPlaybackLabelSizeAndPosition);
            CameraManUIConfigChanges.ApplyConfigChangesForSizeAndPosition(_trackResolutionInput, Config.UI.TrackResolutionSizeAndPosition);
            CameraManUIConfigChanges.ApplyConfigChangesForSizeAndPosition(_trackResolutionLabel, Config.UI.TrackResolutionLabelSizeAndPosition);
            CameraManUIConfigChanges.ApplyConfigChangesForSizeAndPosition(_smoothingStepsInput, Config.UI.SmoothingStepsSizeAndPosition);
            CameraManUIConfigChanges.ApplyConfigChangesForSizeAndPosition(_smoothingStepsLabel, Config.UI.SmoothingStepsLabelSizeAndPosition);
            CameraManUIConfigChanges.ApplyConfigChangesForSizeAndPosition(_showPanelDuringPlaybackCheckbox, Config.UI.ShowPanelDuringPlaybackSizeAndPosition);
            CameraManUIConfigChanges.ApplyConfigChangesForSizeAndPosition(_showPanelDuringPlaybackCheckboxLabel, Config.UI.ShowPanelDuringPlaybackLabelSizeAndPosition);
        }

        public void ShowUI(bool value)
        {
            _panelObj.SetActive(value);
            GUIManager.BlockInput(value);
        }
    }
}