using System;
using UnityEditor;
using UnityEngine;

namespace Kinogoblin.Editor
{
    public class KinogoblinUIRenderer
    {
        private const int SIDEBAR_WIDTH = 150;
        private const float SPACE_SIZE = 10f;

        private readonly EditorWindowState _state;
        private GUIStyle _style;

        public KinogoblinUIRenderer(EditorWindowState state)
        {
            _state = state;
        }

        public void DrawUI(Rect windowRect)
        {
            DrawBackground(windowRect);
            DrawHeader();
            DrawMainContent();
        }

        private void DrawBackground(Rect windowRect)
        {
            if (Other.customView)
            {
                Rect graphPosition = new Rect(0f, 0f, windowRect.width, windowRect.height);
                GraphBackground.DrawGraphBackground(graphPosition, graphPosition);
            }
        }

        private void DrawHeader()
        {
            _style = "box";
            GUILayout.Space(SPACE_SIZE);
            GUILayout.Box("KINOGOBLIN EDITOR", Other.HeaderStyle,
                GUILayout.ExpandWidth(true));
        }

        private void DrawMainContent()
        {
            EditorGUILayout.BeginHorizontal();

            DrawSidebar();
            DrawContentArea();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawSidebar()
        {
            using (new EditorGUILayout.VerticalScope(_style,
                       GUILayout.MaxWidth(SIDEBAR_WIDTH), GUILayout.ExpandHeight(true)))
            {
                var newIndex = GUILayout.SelectionGrid((int)_state.currentSettingType, SettingsRegistry.DisplayNames, 1);
                var newType = (TypesOfSettings)newIndex;
                _state.ResetScrollOnTypeChange(newType);
            }
        }

        private void DrawContentArea()
        {
            using (new EditorGUILayout.VerticalScope(_style, GUILayout.ExpandHeight(true)))
            {
                using (var scrollView = new EditorGUILayout.ScrollViewScope(_state.scrollPosition))
                {
                    _state.scrollPosition = scrollView.scrollPosition;
                    DrawSettingsContent();
                }
            }
        }

        private void DrawSettingsContent()
        {
            try
            {
                var setting = SettingsRegistry.GetSetting(_state.currentSettingType);
                setting.GuiAction?.Invoke();
            }
            catch (Exception ex)
            {
                DrawErrorContent(ex);
            }
        }

        private void DrawErrorContent(Exception ex)
        {
            EditorGUILayout.HelpBox($"Error drawing {_state.currentSettingType} settings: {ex.Message}",
                MessageType.Error);

            if (GUILayout.Button("Reset to Default"))
            {
                _state.currentSettingType = TypesOfSettings.FolderUtils;
                _state.scrollPosition = Vector2.zero;
            }
        }
    }
}