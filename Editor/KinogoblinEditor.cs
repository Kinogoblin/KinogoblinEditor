using System;
using UnityEngine;
using UnityEditor;

namespace Kinogoblin.Editor
{
    #region Enums and Data

    public enum TypesOfSettings
    {
        FolderUtils,
        SettingsForGameObject,
        ChangeMaterial,
        AddOtherPackages,
        Timeline,
        SceneFastTravel,
        BuildSettings,
        Custom
    }

    [Serializable]
    public class EditorWindowState
    {
        public TypesOfSettings currentSettingType;
        public Vector2 scrollPosition = Vector2.zero;

        public void ResetScrollOnTypeChange(TypesOfSettings newType)
        {
            if (newType != currentSettingType)
            {
                scrollPosition = Vector2.zero;
                currentSettingType = newType;
            }
        }
    }

    #endregion

    public sealed class KinogoblinEditorWindow : EditorWindow
    {
        [MenuItem("Tools/Kinogoblin tools/Set Settings Window", false, -100)]
        public static void ShowWindow()
        {
            KinogoblinWindowManager.ShowWindow();
        }

        [MenuItem("Tools/Kinogoblin tools/HotKeys/Set Settings Window #w", false, -100)]
        public static void ShowWindowHotKey()
        {
            KinogoblinWindowManager.ShowWindowHotKey();
        }

        private readonly EditorWindowState _state = new();
        private KinogoblinUIRenderer _uiRenderer;

        private void OnEnable()
        {
            _uiRenderer = new KinogoblinUIRenderer(_state);
            titleContent = new GUIContent("K. Editor", KinogoblinResources.GetLogo());
            Initialize();
        }

        private void OnGUI()
        {
            _uiRenderer.DrawUI(position);
        }

        private void OnDestroy()
        {
            KinogoblinResources.ClearCache();
        }

        private static void Initialize()
        {
            try
            {
                Other.UpdateScriptableObj();
                FolderUtils.UpdateScriptableObj();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[KinogoblinEditor] Error during initialization: {ex.Message}");
            }
        }
    }
}