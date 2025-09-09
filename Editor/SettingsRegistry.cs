using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Kinogoblin.Editor
{
    public static class SettingsRegistry
    {
        private static readonly Dictionary<TypesOfSettings, SettingInfo> Settings = new();
        private static string[] _cachedDisplayNames;

        static SettingsRegistry()
        {
            RegisterDefaultSettings();
            CacheDisplayNames();
        }

        private static void RegisterDefaultSettings()
        {
            Register(TypesOfSettings.ChangeMaterial, "Change Material", ChangeMaterialWindow.ChangeMaterialOnGUI);
            Register(TypesOfSettings.SettingsForGameObject, "Settings For GameObject",
                SettingsForGameobject.SettingsForGameObjectGUI);
            Register(TypesOfSettings.FolderUtils, "Folder Utils", FolderUtils.FolderUtilsGUI);
            Register(TypesOfSettings.AddOtherPackages, "Add Other Packages", OtherPackages.OtherPackagesGUI);
            Register(TypesOfSettings.Custom, "Custom", Other.OtherGUI);
            Register(TypesOfSettings.Timeline, "Timeline", Timeline.TimelineUtilities.TimelineUtilitiesGUI);
            Register(TypesOfSettings.SceneFastTravel, "Scene Fast Travel", SceneFastTravel.SceneFastTravelGUI);
            Register(TypesOfSettings.BuildSettings, "Build Settings", BuildSettingsGUI.BuildSettingsGUIWindow);
        }

        private static void Register(TypesOfSettings type, string displayName, Action guiAction)
        {
            Settings[type] = new SettingInfo(displayName, guiAction);
        }

        private static void CacheDisplayNames()
        {
            _cachedDisplayNames = Enum.GetValues(typeof(TypesOfSettings))
                .Cast<TypesOfSettings>()
                .Select(t => Settings.TryGetValue(t, out var s) ? s.DisplayName : t.ToString())
                .ToArray();
        }
        
        public static SettingInfo GetSetting(TypesOfSettings type)
        {
            if (Settings.TryGetValue(type, out var setting))
                return setting;
            throw new ArgumentException($"Setting type '{type}' is not registered", nameof(type));
        }

        public static string[] DisplayNames => _cachedDisplayNames;

        public readonly struct SettingInfo
        {
            public readonly string DisplayName;
            public readonly Action GuiAction;

            public SettingInfo(string displayName, Action guiAction)
            {
                DisplayName = displayName;
                GuiAction = guiAction;
            }
        }
    }
}