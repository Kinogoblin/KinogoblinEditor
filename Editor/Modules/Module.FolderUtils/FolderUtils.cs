using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Kinogoblin.Editor.FavoriteAssets;

namespace Kinogoblin.Editor
{

    public class FolderUtils : EditorWindow
    {
        private static bool _smallSettings = false;
        
        public static void FolderUtilsGUI()
        {
            ScriptableObject scriptableObj = ProfileData.Instance;
            SerializedObject serialObj = new SerializedObject(scriptableObj);
            SerializedProperty serialProp = serialObj.FindProperty("sceneHierarchy");
            SerializedProperty serialPropFolder = serialObj.FindProperty("customFolderHierarchy");


            GUILayout.Label("Scene utils", EditorStyles.boldLabel);

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

            GUILayout.Space(10f);

            _smallSettings = GUILayout.Toggle(_smallSettings, "Choose small variant");
            if (GUILayout.Button("Create Folder Catalog"))
            {
                CreateProjectsComponents.FolderCreate(_smallSettings);
            }
            if (GUILayout.Button("Create Folder Catalog under active folder"))
            {
                CreateProjectsComponents.FolderCreateUnderActive(CreateProjectsComponents.GetSelectedPathOrFallback(), _smallSettings);
            }

            serialObj.ApplyModifiedProperties();
        }
    }
    static class CreateProjectsComponents
    {
        [MenuItem("Tools/Kinogoblin tools/Shortcuts/Create Scene Catalog")]
        public static void SceneCreate()
        {
            Helpful.Debug("Kinogoblin Editor ", " Create scene catalog");
            var sceneGONames = ProfileData.Instance.sceneHierarchy.sceneGONames;
            foreach (var item in sceneGONames)
            {
                var newGO = new GameObject(item).transform;
            }
        }

        [MenuItem("Tools/Kinogoblin tools/Shortcuts/Create Folder Catalog #f")]
        public static void FolderCreate()
        {
            Helpful.Debug("Kinogoblin Editor ", " Create folder catalog");
            var projectName = ProfileData.Instance.customFolderHierarchy.projectName;
            var projectFolders = ProfileData.Instance.customFolderHierarchy.paths;
            if (!Directory.Exists(projectName))
            {
                Directory.CreateDirectory(projectName);
            }
            foreach (var item in projectFolders)
            {
                Directory.CreateDirectory(projectName + item);
            }
            AssetDatabase.Refresh();
        }

        public static void FolderCreate(bool small)
        {
            Helpful.Debug("Kinogoblin Editor ", " Create folder catalog");
            var projectName = ProfileData.Instance.customFolderHierarchy.projectName;
            if (!Directory.Exists(projectName))
            {
                Directory.CreateDirectory(projectName);
            }
            var foldersPath = ProfileData.Instance.customFolderHierarchy.paths;
            if (small)
            {
                foldersPath = ProfileData.Instance.customFolderHierarchy.pathsSmallVersion;
            }
            foreach (var item in foldersPath)
            {
                Directory.CreateDirectory(projectName + item);
            }
            AssetDatabase.Refresh();
        }

        public static void FolderCreateUnderActive(string path, bool small)
        {
            Helpful.Debug("Kinogoblin Editor ", " Create folder catalog");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var foldersPath = ProfileData.Instance.customFolderHierarchy.paths;
            if (small)
            {
                foldersPath = ProfileData.Instance.customFolderHierarchy.pathsSmallVersion;
            }
            foreach (var item in foldersPath)
            {
                Directory.CreateDirectory(path + item);
            }
            AssetDatabase.Refresh();
        }

        public static string GetSelectedPathOrFallback()
        {
            string path = "Assets";

            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            return path;
        }
    }
}