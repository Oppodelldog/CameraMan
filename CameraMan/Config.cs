using System.Diagnostics;
using System.IO;
using BepInEx.Configuration;

namespace CameraMan
{
    public static class Config
    {
        private static ConfigFile _pluginConfig;
        private static FileSystemWatcher _configWatcher;

        public static class Visibility
        {
            internal static ConfigEntry<bool> ShowTrackMarkers;
            internal static ConfigEntry<bool> ShowPlaybackTrackMarkers;
            internal static ConfigEntry<bool> ShowTrackMarkersDuringPlayback;
            internal static ConfigEntry<bool> ShowPlaybackTrackMarkersDuringPlayback;
            internal static ConfigEntry<bool> ShowPanelDuringPlayback;
        }

        public static class Settings
        {
            internal static ConfigEntry<int> TrackResolution;
            internal static ConfigEntry<int> SmoothingSteps;
            internal static ConfigEntry<float> PlaybackSpeed;
            internal static ConfigEntry<float> PlaybackSpeedMultiplier;
            internal static ConfigEntry<bool> EnableConfigFileWatcher;
        }

        public static class UI
        {
            internal static ConfigEntry<int> TitleFontSize;
            internal static ConfigEntry<int> LabelFontSize;
            internal static ConfigEntry<int> DropDownFontSize;
            internal static ConfigEntry<int> InputFieldFontSize;

            internal static ConfigEntry<string> PanelSizeAndPosition;

            internal static ConfigEntry<string> PanelTitleSizeAndPosition;
            internal static ConfigEntry<string> SpeedMultiplierSizeAndPosition;
            internal static ConfigEntry<string> SpeedMultiplierLabelSizeAndPosition;

            internal static ConfigEntry<string> ShowTrackMarkersSizeAndPosition;
            internal static ConfigEntry<string> ShowTrackMarkersLabelSizeAndPosition;

            internal static ConfigEntry<string> ShowPlaybackTrackMarkersSizeAndPosition;
            internal static ConfigEntry<string> ShowPlaybackTrackMarkersLabelSizeAndPosition;

            internal static ConfigEntry<string> ShowTrackMarkersDuringPlaybackSizeAndPosition;
            internal static ConfigEntry<string> ShowTrackMarkersDuringPlaybackLabelSizeAndPosition;

            internal static ConfigEntry<string> ShowPlaybackTrackMarkersDuringPlaybackSizeAndPosition;
            internal static ConfigEntry<string> ShowPlaybackTrackMarkersDuringPlaybackLabelSizeAndPosition;

            internal static ConfigEntry<string> TrackResolutionSizeAndPosition;
            internal static ConfigEntry<string> TrackResolutionLabelSizeAndPosition;

            internal static ConfigEntry<string> SmoothingStepsSizeAndPosition;
            internal static ConfigEntry<string> SmoothingStepsLabelSizeAndPosition;

            internal static ConfigEntry<string> ShowPanelDuringPlaybackSizeAndPosition;
            internal static ConfigEntry<string> ShowPanelDuringPlaybackLabelSizeAndPosition;
        }

        internal static void Init(Plugin plugin)
        {
            Visibility.ShowTrackMarkers = plugin.Config.Bind("visibility", "Show Track Markers", true, "Show the markers placed by the user");
            Visibility.ShowPlaybackTrackMarkers = plugin.Config.Bind("visibility", "Show Playback Track Markers", true, "Show the markers generated for playback");
            Visibility.ShowTrackMarkersDuringPlayback = plugin.Config.Bind("visibility", "Show Track Markers During Playback", true, "Show the markers placed by the user during playback");
            Visibility.ShowPlaybackTrackMarkersDuringPlayback = plugin.Config.Bind("visibility", "Show Playback Track Markers During Playback", true, "Show the markers generated for playback during playback");
            Visibility.ShowPanelDuringPlayback = plugin.Config.Bind("visibility", "Show Panel During Playback", true, "Show the CameraMan panel during playback");

            Settings.TrackResolution = plugin.Config.Bind("settings", "Track Resolution", 20, "The number of markers generated for smoothing");
            Settings.SmoothingSteps = plugin.Config.Bind("settings", "Smoothing Steps", 20, "The number smoothing iterations applied to the track");
            Settings.PlaybackSpeed = plugin.Config.Bind("settings", "Playback Speed", 0.5f, "The speed (0-1) at which the camera moves along the track");
            Settings.PlaybackSpeedMultiplier = plugin.Config.Bind("settings", "Playback Speed Multiplier", 1f, "A multiplier on the configured playback speed");
            Settings.EnableConfigFileWatcher = plugin.Config.Bind("settings", "Enable Config File Watcher", false, "Reload the config when the config file changes");

            UI.TitleFontSize = plugin.Config.Bind("ui", "Title Font Size", 20, "The font size of the CameraMan UI title");
            UI.LabelFontSize = plugin.Config.Bind("ui", "Label Font Size", 14, "The font size of the CameraMan UI labels");
            UI.DropDownFontSize = plugin.Config.Bind("ui", "Drop Down Font Size", 12, "The font size of the CameraMan UI drop down menus");
            UI.InputFieldFontSize = plugin.Config.Bind("ui", "Input Field Font Size", 12, "The font size of the CameraMan UI input fields");

            UI.PanelSizeAndPosition = plugin.Config.Bind("ui", "Panel Size And Position", "600,300,10,0", "The size and position of the CameraMan UI");
            UI.PanelTitleSizeAndPosition = plugin.Config.Bind("ui", "Panel Title Size And Position", "170,30,0,-12", "The size and position of the CameraMan UI title");
            UI.SpeedMultiplierSizeAndPosition = plugin.Config.Bind("ui", "Speed Multiplier Size And Position", "100,30,100,-45", "The size and position of the CameraMan speed multiplier input field");
            UI.SpeedMultiplierLabelSizeAndPosition = plugin.Config.Bind("ui", "Speed Multiplier Label Size And Position", "100,30,20,-50", "The size and position of the CameraMan speed multiplier label");
            UI.ShowTrackMarkersSizeAndPosition = plugin.Config.Bind("ui", "Show Track Markers Size And Position", "20,20,40,-110", "The size and position of the CameraMan show track markers toggle");
            UI.ShowTrackMarkersLabelSizeAndPosition = plugin.Config.Bind("ui", "Show Track Markers Label Size And Position", "200,50,70,-108", "The size and position of the CameraMan show track markers label");
            UI.ShowPlaybackTrackMarkersSizeAndPosition = plugin.Config.Bind("ui", "Show Playback Track Markers Size And Position", "20,20,40,-145", "The size and position of the CameraMan show playback track markers toggle");
            UI.ShowPlaybackTrackMarkersLabelSizeAndPosition = plugin.Config.Bind("ui", "Show Playback Track Markers Label Size And Position", "300,50,70,-143", "The size and position of the CameraMan show playback track markers label");
            UI.ShowTrackMarkersDuringPlaybackSizeAndPosition = plugin.Config.Bind("ui", "Show Track Markers During Playback Size And Position", "20,20,40,-180", "The size and position of the CameraMan show track markers during playback toggle");
            UI.ShowTrackMarkersDuringPlaybackLabelSizeAndPosition = plugin.Config.Bind("ui", "Show Track Markers During Playback Label Size And Position", "400,50,70,-178", "The size and position of the CameraMan show track markers during playback label");
            UI.ShowPlaybackTrackMarkersDuringPlaybackSizeAndPosition = plugin.Config.Bind("ui", "Show Playback Track Markers During Playback Size And Position", "20,20,40,-215", "The size and position of the CameraMan show playback track markers during playback toggle");
            UI.ShowPlaybackTrackMarkersDuringPlaybackLabelSizeAndPosition = plugin.Config.Bind("ui", "Show Playback Track Markers During Playback Label Size And Position", "500,50,70,-213", "The size and position of the CameraMan show playback track markers during playback label");

            UI.TrackResolutionSizeAndPosition = plugin.Config.Bind("ui", "Track Resolution Size And Position", "50,30,500,-50", "The size and position of the CameraMan track resolution input field");
            UI.TrackResolutionLabelSizeAndPosition = plugin.Config.Bind("ui", "Track Resolution Label Size And Position", "120,50,380,-58", "The size and position of the CameraMan track resolution label");

            UI.SmoothingStepsSizeAndPosition = plugin.Config.Bind("ui", "Smoothing Steps Size And Position", "50,30,500,-80", "The size and position of the CameraMan smoothing steps input field");
            UI.SmoothingStepsLabelSizeAndPosition = plugin.Config.Bind("ui", "Smoothing Steps Label Size And Position", "120,50,380,-88", "The size and position of the CameraMan smoothing steps label");
            
            UI.ShowPanelDuringPlaybackSizeAndPosition = plugin.Config.Bind("ui", "Show Panel During Playback Size And Position", "20,20,350,-143", "The size and position of the CameraMan show panel during playback toggle");
            UI.ShowPanelDuringPlaybackLabelSizeAndPosition = plugin.Config.Bind("ui", "Show Panel During Playback Label Size And Position", "200,50,380,-143", "The size and position of the CameraMan show panel during playback label");

            _pluginConfig = plugin.Config;

            if (Settings.EnableConfigFileWatcher.Value)
            {
                DoEnableConfigFileWatcher(plugin);
            }

            Settings.EnableConfigFileWatcher.SettingChanged += (sender, args) =>
            {
                if ((bool)((SettingChangedEventArgs)args).ChangedSetting.BoxedValue)
                {
                    DoEnableConfigFileWatcher(plugin);
                }
                else
                {
                    DoDisableConfigFileWatcher();
                }
            };
        }

        private static void DoEnableConfigFileWatcher(Plugin plugin)
        {
            var configFilePath = plugin.Config.ConfigFilePath;
            var configFolder = Path.GetDirectoryName(configFilePath);
            var configFilename = Path.GetFileName(configFilePath);

            Debug.Assert(configFolder != null, nameof(configFolder) + " != null");

            Jotunn.Logger.LogInfo($"watching config for changes - folder: {configFolder} filename: {configFilename}");

            _configWatcher = new FileSystemWatcher(configFolder, configFilename);
            _configWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
            _configWatcher.Changed += OnConfigChanged;
            _configWatcher.EnableRaisingEvents = true;
        }

        public static void DoDisableConfigFileWatcher()
        {
            if (_configWatcher == null) return;

            Jotunn.Logger.LogInfo("disabling config file watcher");

            _configWatcher.EnableRaisingEvents = false;
            _configWatcher.Dispose();
            _configWatcher = null;
        }

        private static void OnConfigChanged(object sender, FileSystemEventArgs e)
        {
            Jotunn.Logger.LogInfo($"Config file changed: {e.ChangeType}, reloading config");
            _pluginConfig.Reload();
        }
    }
}