using System;
using UnityEditor;
using UnityEngine;

namespace Kinogoblin.Editor.FavoriteAssets
{
    public class PreferencesWindow : EditorWindow
    {
        private const string WINDOWNAME = "Favorite Assets Preferences";

        public static void Init()
        {
            Vector2 mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            PreferencesWindow m_window = EditorWindow.GetWindowWithRect<PreferencesWindow>(new Rect(110, 110, 300, 500), true, WINDOWNAME);
            m_window.titleContent.image = ConfigData.Instance.SettingsIcon;
            m_window.position = new Rect(mousePos.x - 170, mousePos.y + 70, 250, 400);
        }

        void OnGUI()
        {
            GUILayout.Label(ConfigData.Instance.SettingsIcon);

            Preferences.AutoStart = drawSetting("Start automatically", Preferences.AutoStart, Preferences.InitialValueAutoStart);
            Preferences.KeepHistory = drawSetting("Keep history", Preferences.KeepHistory, Preferences.InitialValueKeepHistory);
            Preferences.DefaultToEnvironmentUser = drawSetting("Default to local user", Preferences.DefaultToEnvironmentUser, Preferences.InitialValueDefaultToEnvironmentUser);
            Preferences.ShowThumbnails = drawSetting("Show thumbnails", Preferences.ShowThumbnails, Preferences.InitialValueShowThumbnails);
            if (Preferences.ShowThumbnails)
                Preferences.ThumbnailSize = drawSetting("ThumbnailSize", Preferences.ThumbnailSize, Preferences.InitialValueThumbnailSize, Preferences.ThumbnailMaxSize, "");

            Preferences.ShowShortcutHint = drawSetting("Show shortcut hint", Preferences.ShowShortcutHint, Preferences.InitialValueShowShortcutHint);
            Preferences.ShowMenuText = drawSetting("Show menu text", Preferences.ShowMenuText, Preferences.InitialValueShowMenuText);

            EditorGUILayout.Space();
            /*string mainWindowStatePrefPrefixAppend = "";
            switch ( Preferences.DefaultMainWindowState)
            {
                case 0:
                    mainWindowStatePrefPrefixAppend = "Favorites";
                    break;
                case 1:
                    mainWindowStatePrefPrefixAppend = "History";
                    break;
                case 2:
                    mainWindowStatePrefPrefixAppend = "Scene Count";
                    break;
            }
             Preferences.DefaultMainWindowState = drawSetting("Default:",  Preferences.DefaultMainWindowState, 0, 2, mainWindowStatePrefPrefixAppend);*/
            Preferences.HistoryCountLimit = drawSetting("History count", Preferences.HistoryCountLimit, 5, 40, "");
            Preferences.SceneCountLimit = drawSetting("Scene count", Preferences.SceneCountLimit, 5, 40, "");

            EditorGUILayout.Space();
            if (GUILayout.Button("Reset all to default"))
            {
                Preferences.AutoStart = Preferences.InitialValueAutoStart;
                Preferences.KeepHistory = Preferences.InitialValueKeepHistory;
                Preferences.DefaultToEnvironmentUser = Preferences.InitialValueDefaultToEnvironmentUser;
                Preferences.ShowThumbnails = Preferences.InitialValueShowThumbnails;
                Preferences.ShowShortcutHint = Preferences.InitialValueShowShortcutHint;
                Preferences.HistoryCountLimit = Preferences.InitialValueHistoryCountLimit;
                Preferences.DefaultMainWindowState = Preferences.DefaultMainWindowState;
                Preferences.SceneCountLimit = Preferences.InitialValueSceneCountLimit;
                Preferences.ShowMenuText = Preferences.InitialValueShowMenuText;
            }
        }

        private int drawSetting(string title, int value, int min, int max, string prefixAppend)
        {
            EditorGUILayout.PrefixLabel(title + prefixAppend);
            return EditorGUILayout.IntSlider(value, min, max);
        }

        private bool drawSetting(string title, bool value, bool defaultVal)
        {
            return EditorGUILayout.ToggleLeft(title, value, (defaultVal != value) ? EditorStyles.boldLabel : EditorStyles.label);
        }
    }
}