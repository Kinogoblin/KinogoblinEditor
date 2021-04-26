using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

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

            if (GUILayout.Button("Cleanup Missing Scripts"))
                CleanupMissingScripts();
                
            GUILayout.Space(10f);

            settings.customView = EditorGUILayout.Toggle("Custom View", settings.customView);
            settings.debugSend = EditorGUILayout.Toggle("Debug send", settings.debugSend);
        }

        #if UNITY_2019_1_OR_NEWER

        [MenuItem("Tools/Kinogoblin tools/Cleanup Missing Scripts")]
        static void CleanupMissingScripts()
        {
            int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;

            for (int a = 0; a < UnityEngine.SceneManagement.SceneManager.sceneCount; a++)
            {

                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(a);

                var rootGameObjects = scene.GetRootGameObjects();

                if (rootGameObjects != null && rootGameObjects.Length > 0)
                {

                    List<GameObject> allObjectsinScene = new List<GameObject>();


                    EditorUtility.DisplayProgressBar("Preprocessing", $"Fetching GameObjects in active scene \"{scene.name}\"", 0);

                    foreach (var gameObject in rootGameObjects)
                    {
                        var childObjects = gameObject.GetComponentsInChildren<Transform>();

                        if (childObjects != null && childObjects.Length > 0)
                        {
                            foreach (var obj in childObjects)
                            {
                                if (obj != null) { allObjectsinScene.Add(obj.gameObject); }
                            }
                        }

                    }

                    EditorUtility.ClearProgressBar();


                    for (int b = 0; b < allObjectsinScene.Count; b++)
                    {

                        var gameObject = allObjectsinScene[b];

                        EditorUtility.DisplayProgressBar("Removing missing script references", $"Inspecting GameObject  {b + 1}/{allObjectsinScene.Count} in active scene \"{scene.name}\"", (float)(b) / allObjectsinScene.Count);

                        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
                    }

                    EditorSceneManager.MarkSceneDirty(scene);

                    EditorUtility.ClearProgressBar();
                }

                EditorUtility.ClearProgressBar();
            }

            EditorUtility.ClearProgressBar();

            EditorUtility.DisplayDialog("Operation Completed", "Successfully removed missing script references. Please save all currently open scenes to keep these changes persistent", "Ok");

        }

#endif 
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
