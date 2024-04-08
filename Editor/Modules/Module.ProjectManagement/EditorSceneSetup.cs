using UnityEngine;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditor;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using System;
using UnityEditor.Callbacks;
using UnityEditor.ProjectWindowCallback;
using System.IO;
using Kinogoblin.Editor.FavoriteAssets;

namespace Kinogoblin
{
    public class EditorSceneSetup : ScriptableObject
    {
        [MenuItem("Tools/Kinogoblin tools/Save Scene Setup As...", priority = 170)]
        public static void SaveSetup()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save EditorSceneSetup", "New EditorSceneSetup", "asset",
                "Save EditorSceneSetup?");
            if (path != string.Empty)
            {
                EditorSceneSetup setup = GetCurrentSetup();
                AssetDatabase.CreateAsset(setup, path);
            }
        }

        [MenuItem("Tools/Kinogoblin tools/HotKeys/Save Scene Setup As... #%&S", priority = 170)]
        public static void SaveSetup_HotKey()
        {
            if (ProfileData.Instance.listenHotKeys)
                SaveSetup();
        }

        public delegate void EditorSceneSetupLoadedDelegate(EditorSceneSetup setup);

        public static event EditorSceneSetupLoadedDelegate onSetupLoaded;

        [OnOpenAsset]
        static bool OnOpenAsset(int instanceID, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID);
            if (obj is EditorSceneSetup)
            {
                EditorSceneSetup setup = (EditorSceneSetup)obj;
                int active = setup.ActiveScene;

                try
                {
                    EditorUtility.DisplayProgressBar("Loading Scenes",
                        string.Format("Loading Scene Setup {0}....", setup.name), 1.0f);
                    RestoreSetup(setup);
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }

                return true;
            }

            return false;
        }

        // [MenuItem("Assets/Create/KinogoblinAssets/Editor Scene Setup", priority = 200)]
        static void CreateAsset()
        {
            AssetFactory.CreateAssetInProjectWindow<EditorSceneSetup>("SceneSet Icon", "New SceneSetup.asset");
        }

        [HideInInspector] public int ActiveScene;
        [HideInInspector] public EditorScene[] LoadedScenes;

        [System.Serializable]
        public struct EditorScene
        {
            public SceneAsset Scene;
            public bool Loaded;
        }

        public static EditorSceneSetup GetCurrentSetup()
        {
            var scenesetups = EditorSceneManager.GetSceneManagerSetup();

            var editorSetup = CreateInstance<EditorSceneSetup>();

            int i = 0;
            editorSetup.LoadedScenes = new EditorScene[scenesetups.Length];
            foreach (var setup in scenesetups)
            {
                if (setup.isActive)
                    editorSetup.ActiveScene = i;

                editorSetup.LoadedScenes[i].Scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(setup.path);
                editorSetup.LoadedScenes[i].Loaded = setup.isLoaded;

                i++;
            }

            return editorSetup;
        }

        public static void RestoreSetup(EditorSceneSetup editorSetup)
        {
            SceneSetup[] setups = new SceneSetup[editorSetup.LoadedScenes.Length];

            for (int i = 0; i < setups.Length; i++)
            {
                setups[i] = new SceneSetup();
                string path = AssetDatabase.GetAssetPath(editorSetup.LoadedScenes[i].Scene);
                setups[i].path = path;
                setups[i].isLoaded = editorSetup.LoadedScenes[i].Loaded;
                setups[i].isActive = (editorSetup.ActiveScene == i);
            }

            EditorSceneManager.RestoreSceneManagerSetup(setups);

            if (onSetupLoaded != null)
                onSetupLoaded.Invoke(editorSetup);
        }
    }

    public class AssetFactory
    {
        public static void CreateAssetInProjectWindow<T>(string iconName, string fileName) where T : ScriptableObject
        {
            var icon = EditorGUIUtility.FindTexture(iconName);

            var namingInstance = new DoCreateGenericAsset();
            namingInstance.type = typeof(T);
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, namingInstance, fileName, icon, null);
        }

        public static ScriptableObject CreateAssetAtPath(string path, Type type)
        {
            Debug.Log("CreateAssetAtPath (" + type.Name + ")");

            ScriptableObject asset = ScriptableObject.CreateInstance(type);
            asset.name = Path.GetFileName(path);
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        class DoCreateGenericAsset : EndNameEditAction
        {
            public Type type;

            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                ScriptableObject asset = AssetFactory.CreateAssetAtPath(pathName, type);
                ProjectWindowUtil.ShowCreatedAsset(asset);
            }
        }
    }
}