using Kinogoblin.Editor.FavoriteAssets;
using UnityEditor;
using UnityEngine;

namespace Kinogoblin.Editor
{
    public class BuildSettings : EditorWindow
    {
        private static readonly GUILayoutOption headerHeight = GUILayout.Height(25);
        private static ScriptableObject scriptableObj;
        private static SerializedObject serialObj;
        private static SerializedProperty serialProp;
        private static SerializedProperty serialPropFolder;
        private static GUIStyle headerStyle;

        public static void BuildSettingsGUI()
        {
            if (scriptableObj == null)
            {
                scriptableObj = ProfileData.Instance;
                serialObj = new SerializedObject(scriptableObj);
                serialProp = serialObj.FindProperty("sceneHierarchy");
                serialPropFolder = serialObj.FindProperty("customFolderHierarchy");
            }
            
            if (headerStyle == null)
            {
                headerStyle = new GUIStyle(GUI.skin.box)
                    { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
            }
            
            GUILayout.Box("SCENE AND FOLDER UTILS", headerStyle, GUILayout.ExpandWidth(true), headerHeight);

            GUILayout.Space(10f);

            EditorGUILayout.PropertyField(serialProp, true);

            GUILayout.Space(10f);

            if (GUILayout.Button("Create Scene Catalog"))
            {
                CreateProjectsComponents.SceneCreate();
            }

            GUILayout.Space(10f);
            GUILayout.Label("Folder utils", EditorStyles.boldLabel);
            GUILayout.Space(10f);

            EditorGUILayout.PropertyField(serialPropFolder, true);
            serialObj.ApplyModifiedProperties();

            GUILayout.Space(10f);

            serialObj.ApplyModifiedProperties();
        }

        public static void UpdateScriptableObj()
        {
            scriptableObj = ProfileData.Instance;
            serialObj = new SerializedObject(scriptableObj);
            serialProp = serialObj.FindProperty("sceneHierarchy");
            serialPropFolder = serialObj.FindProperty("customFolderHierarchy");
        }
    }
}