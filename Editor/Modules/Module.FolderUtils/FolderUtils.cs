using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Kinogoblin
{

    public class FolderUtils : EditorWindow
    {
        private static bool _smallSettings = false;
        
        public static void FolderUtilsGUI()
        {
            GUILayout.Label("Scene utils", EditorStyles.boldLabel);
            GUILayout.Space(10f);
            if (GUILayout.Button("Create Scene Catalog"))
            {
                CreateProjectsComponents.SceneCreate();
            }
            GUILayout.Space(10f);
            GUILayout.Label("Folder utils", EditorStyles.boldLabel);
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
        }
    }
    static class CreateProjectsComponents
    {
        [MenuItem("Tools/Kinogoblin tools/Shortcuts/Create Scene Catalog #s")]
        public static void SceneCreate()
        {
            Helpful.Debug("Kinogoblin Editor ", " Create scene catalog");
            var cameraObj = new GameObject("p--Player").transform;
            var scriptObj = new GameObject("m--Managers").transform;
            var lightObj = new GameObject("l--Light").transform;
            var staticObj = new GameObject("e--Enviroment").transform;
            var dinamicObj = new GameObject("i--Interactable").transform;
            var audioObj = new GameObject("s--Sound").transform;
            var timelines = new GameObject("t--Timelines").transform;
        }

        [MenuItem("Tools/Kinogoblin tools/Shortcuts/Create Folder Catalog #f")]
        public static void FolderCreate()
        {
            Helpful.Debug("Kinogoblin Editor ", " Create folder catalog");
            string path = "Assets/__Project__";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            Directory.CreateDirectory(path + "/Materials");
            Directory.CreateDirectory(path + "/Prefabs");
            Directory.CreateDirectory(path + "/Data");
            Directory.CreateDirectory(path + "/Scripts");
            Directory.CreateDirectory(path + "/Scenes");
            Directory.CreateDirectory(path + "/Trash");
            Directory.CreateDirectory(path + "/Animations");
            Directory.CreateDirectory(path + "/Animations/AnimationClips");
            Directory.CreateDirectory(path + "/Animations/Timelines");
            Directory.CreateDirectory(path + "/Editor");
            Directory.CreateDirectory(path + "/Audio");
            Directory.CreateDirectory(path + "/Models");
            Directory.CreateDirectory(path + "/Materials/Textures");
            Directory.CreateDirectory(path + "/Materials/Shaders");
            AssetDatabase.Refresh();
        }

        public static void FolderCreate(bool small)
        {
            Helpful.Debug("Kinogoblin Editor ", " Create folder catalog");
            string path = "Assets/__Project__";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            Directory.CreateDirectory(path + "/Materials");
            Directory.CreateDirectory(path + "/Prefabs");
            Directory.CreateDirectory(path + "/Scripts");
            Directory.CreateDirectory(path + "/Scenes");
            Directory.CreateDirectory(path + "/Trash");
            if (!small)
            {
                Directory.CreateDirectory(path + "/Animations");
                Directory.CreateDirectory(path + "/Animations/AnimationClips");
                Directory.CreateDirectory(path + "/Animations/Timelines");
                Directory.CreateDirectory(path + "/Editor");
                Directory.CreateDirectory(path + "/Audio");
                Directory.CreateDirectory(path + "/Models");
                Directory.CreateDirectory(path + "/Materials/Textures");
                Directory.CreateDirectory(path + "/Materials/Shaders");
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
            Directory.CreateDirectory(path + "/Prefabs");
            Directory.CreateDirectory(path + "/Scripts");
            Directory.CreateDirectory(path + "/Scenes");
            Directory.CreateDirectory(path + "/Trash");
            if (!small)
            {
                Directory.CreateDirectory(path + "/Animations");
                Directory.CreateDirectory(path + "/Animations/Timelines");
                Directory.CreateDirectory(path + "/Editor");
                Directory.CreateDirectory(path + "/Animations/AnimationClips");
                Directory.CreateDirectory(path + "/Audio");
                Directory.CreateDirectory(path + "/Models");
                Directory.CreateDirectory(path + "/Materials");
                Directory.CreateDirectory(path + "/Scripts/Managers");
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