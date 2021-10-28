using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kinogoblin.Editor.FavoriteAssets
{
    public static class Preferences
    {
        public const string QSPrefsMainWindowState = " .MainWindowState";
        public const string QSPrefsHistorySortState = " .HistorySortState";

        public const string QSPrefsAutoStart = " .AutoStart";
        public const string QSPrefsKeepHistory = " .KeepHistory";
        public const string QSPrefsHistory = " .History";
        //public const string QSPrefsHistoryCount = " .HistoryCount";
        public const string QSPrefsUserOverride = " .UserOverride";
        public const string QSPrefsDefaultToEnvironmentUser = " .DefaultToEnvironmentUser";
        public const string QSPrefsSceneCountLimit = " .SceneCountLimit";
        public const string QSPrefsHistoryCountLimit = " .QSPrefsHistoryCountLimit";
        public const string QSPrefsShowThumbnails = " .ShowThumbnails";
        public const string QSPrefsShowHint = " .ShowHint";
        public const string QSPrefsShowMenuText = " .ShowMenuText";
        public const string QSPrefsThumbnailSize = " .ThumbnailSize";

        internal const bool InitialValueKeepHistory = true;
        internal const bool InitialValueDefaultToEnvironmentUser = true;
        internal const bool InitialValueAutoStart = false;
        internal const bool InitialValueShowThumbnails = true;
        internal const bool InitialValueShowShortcutHint = true;
        internal const bool InitialValueShowMenuText = false;
        internal const int InitialMainWindowState = 1;
        internal const int InitialValueHistoryCountLimit = 20;
        internal const int InitialValueSceneCountLimit = 20;
        internal const int InitialValueThumbnailSize = ThumbnailMinSize;

        public const int ThumbnailMinSize = 16;
        public const int ThumbnailMaxSize = 128;

        public static bool AutoStart
        {
            get { return (EditorPrefs.GetBool(QSPrefsAutoStart)); }
            internal set { EditorPrefs.SetBool(QSPrefsAutoStart, value); }
        }

        public static bool KeepHistory
        {
            get { return ((!EditorPrefs.HasKey(QSPrefsKeepHistory) && InitialValueKeepHistory) || EditorPrefs.GetBool(QSPrefsKeepHistory)); }
            internal set { EditorPrefs.SetBool(QSPrefsKeepHistory, value); }
        }

        public static bool DefaultToEnvironmentUser
        {
            get { return ((!EditorPrefs.HasKey(QSPrefsDefaultToEnvironmentUser) && InitialValueDefaultToEnvironmentUser) || EditorPrefs.GetBool(QSPrefsDefaultToEnvironmentUser)); }
            internal set { EditorPrefs.SetBool(QSPrefsDefaultToEnvironmentUser, value); }
        }

        public static bool ShowThumbnails
        {
            get { return ((!EditorPrefs.HasKey(QSPrefsShowThumbnails) && InitialValueShowThumbnails) || EditorPrefs.GetBool(QSPrefsShowThumbnails)); }
            internal set { EditorPrefs.SetBool(QSPrefsShowThumbnails, value); }
        }

        public static bool ShowShortcutHint
        {
            get { return ((!EditorPrefs.HasKey(QSPrefsShowHint) && InitialValueShowShortcutHint) || EditorPrefs.GetBool(QSPrefsShowHint)); }
            internal set { EditorPrefs.SetBool(QSPrefsShowHint, value); }
        }

        public static bool ShowMenuText
        {
            get { return ((!EditorPrefs.HasKey(QSPrefsShowMenuText) && InitialValueShowMenuText) || EditorPrefs.GetBool(QSPrefsShowMenuText)); }
            internal set { EditorPrefs.SetBool(QSPrefsShowMenuText, value); }
        }

        public static int SceneCountLimit
        {
            get { return (EditorPrefs.HasKey(QSPrefsSceneCountLimit) ? EditorPrefs.GetInt(QSPrefsSceneCountLimit) : InitialValueSceneCountLimit); }
            internal set { EditorPrefs.SetInt(QSPrefsSceneCountLimit, value); }
        }

        public static int HistoryCountLimit
        {
            get { return (EditorPrefs.HasKey(QSPrefsHistoryCountLimit) ? EditorPrefs.GetInt(QSPrefsHistoryCountLimit) : InitialValueHistoryCountLimit); }
            internal set { EditorPrefs.SetInt(QSPrefsHistoryCountLimit, value); }
        }

        public static int ThumbnailSize
        {
            get { return (EditorPrefs.HasKey(QSPrefsThumbnailSize) ? EditorPrefs.GetInt(QSPrefsThumbnailSize) : InitialValueThumbnailSize); }
            internal set { EditorPrefs.SetInt(QSPrefsThumbnailSize, value); }
        }

        public static int DefaultMainWindowState
        {
            get { return (EditorPrefs.HasKey(QSPrefsMainWindowState) ? EditorPrefs.GetInt(QSPrefsMainWindowState) : InitialMainWindowState); }
            internal set { EditorPrefs.SetInt(QSPrefsMainWindowState, value); }
        }

        internal static void SaveHistory(List<string> IDs)
        {
            EditorPrefs.SetString(QSPrefsHistory, Serializer.Serialize(IDs));
        }

        internal static List<String> LoadHistory()
        {
            return Serializer.DeserializeStringList(EditorPrefs.GetString(QSPrefsHistory));
        }

        internal static void SetUserOverride(string userName)
        {
            EditorPrefs.SetString(QSPrefsUserOverride, userName);
        }

        internal static string GetUserOverride()
        {
            return EditorPrefs.GetString(QSPrefsUserOverride);
        }

        internal static void ClearUserOverride()
        {
            EditorPrefs.SetString(QSPrefsUserOverride, "");
        }
    }
}