using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Kinogoblin
{

    public class Other
    {
        public static SaveSettings settings
        {
            get
            {
                if (_dataSettings == null)
                {
                    _dataSettings = EditorSettings.GetSettings();
                }
                return _dataSettings;
            }
        }
        private static SaveSettings _dataSettings;


        public static Color hierarchyColor = new Color(0.5f, 0, 1);

        public static GUIStyle buttonStyle;
        public static readonly GUILayoutOption headerHeight = GUILayout.Height(25);
        public static GUIStyle headerStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter };

        public static bool customView = true;
        public static bool debugSend = true;

        public static void OtherGUI()
        {

            ScriptableObject scriptableObj = settings;
            SerializedObject serialObj = new SerializedObject(scriptableObj);
            SerializedProperty serialProp = serialObj.FindProperty("customHierarchy");

            ///////////////
            GUILayout.Box("COLOR SETTINGS", headerStyle, GUILayout.ExpandWidth(true), headerHeight);

            GUILayout.Space(10f);

            EditorGUILayout.PropertyField(serialProp, true);
            serialObj.ApplyModifiedProperties();

            GUILayout.Space(10f);

            settings.debugColor = EditorGUILayout.ColorField("Color debug", settings.debugColor);

            if (GUILayout.Button("Test Debug color"))
                Helpful.Debug("Hello from Kinogoblin!");

            GUILayout.Space(10f);

            if (GUILayout.Button("Load Kinogoblin layout"))
                LayoutLoader.LoadKinogoblinLayout();

            GUILayout.Space(10f);

            settings.customView = EditorGUILayout.Toggle("Custom View", settings.customView);
            settings.debugSend = EditorGUILayout.Toggle("Debug send", settings.debugSend);
        }
    }
#if UNITY_EDITOR
    public class EditorExtensions
    {
        [MenuItem("Assets/Add Folder to .gitignore")]
        static void AddFolderToGitignore()
        {
            var ignoreFile = Path.Combine(Application.dataPath, @"..\", ".gitignore");
            Debug.Log($"Constructed path: {ignoreFile}");

            if (!File.Exists(ignoreFile))
            {
                Debug.LogError($"[Editor Extenstions] .gitignore file not found. Operation aborted");
                return;
            }

            var selection = Selection.activeObject;

            if (selection == null) return;

            var selectionPath = AssetDatabase.GetAssetPath(selection.GetInstanceID());

            if (selectionPath.Length < 0 || !Directory.Exists(selectionPath))
            {
                Debug.LogError("[Editor Extensions] Selected object is not a folder. Operation aborted");
                return;
            }

            using (var writer = new StreamWriter(ignoreFile, true))
            {
                writer.WriteLine($"{selectionPath}/");
                writer.WriteLine($"{selectionPath}*.*meta");

                Helpful.Debug("Kinogoblin Editor ", $"Folder {selection.name} successfully added to .gitignore");
            }
        }
    }

    [InitializeOnLoad]
    public static class HierarchyWindowGroupHeader
    {
        static HierarchyWindowGroupHeader()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }

        static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject != null && Other.settings.customizeHierarchy)
            {
                foreach (var item in Other.settings.customHierarchy)
                {
                    if (gameObject.name.StartsWith(item.prefix, System.StringComparison.Ordinal) && item.prefix != "")
                    {
                        EditorGUI.DrawRect(selectionRect, item.color);
                        EditorGUI.DropShadowLabel(selectionRect, gameObject.name.Replace(item.prefix, "").ToUpperInvariant(), item.style);
                        return;
                    }
                }
            }

        }
    }
#endif

}
