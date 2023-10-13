using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Kinogoblin.Editor
{
    public class SceneFastTravel : EditorWindow
    {
        private static List<string> _scenePaths = new();
        private static Vector2 _scrollPosition;
        
        public static void SceneFastTravelGUI()
        {
            if (EditorBuildSettings.scenes.Length != _scenePaths.Count)
            {
                GetScenePaths();
            }

            GUILayout.Label("Scenes Setup", EditorStyles.boldLabel);

            if (GUILayout.Button("Create scene setup Collection", GUILayout.Height(20)))
            {
                EditorSceneSetup.SaveSetup();
            }
            
            GUILayout.Space(10f);
            
            GUILayout.Label("Scenes in Build Settings", EditorStyles.boldLabel);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            GUILayout.Label("Open Scene", EditorStyles.boldLabel);
            for (int i = 0; i < _scenePaths.Count; i++)
            {
                if (GUILayout.Button($"Open {GetSceneNameFromPath(_scenePaths[i])}", GUILayout.Height(20)))
                {
                    OpenScene(_scenePaths[i]);
                }
            }
            GUILayout.Label("Add Scene Additional", EditorStyles.boldLabel);
            for (int i = 0; i < _scenePaths.Count; i++)
            {
                if (GUILayout.Button($"Add {GetSceneNameFromPath(_scenePaths[i])}", GUILayout.Height(20)))
                {
                    AddScene(_scenePaths[i]);
                }
            }
            EditorGUILayout.EndScrollView();
        }


        private static void GetScenePaths()
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            _scenePaths = new List<string>();

            for (int i = 0; i < scenes.Length; i++)
            {
                _scenePaths.Add(scenes[i].path);
            }
        }

        private static string GetSceneNameFromPath(string scenePath)
        {
            int startIndex = scenePath.LastIndexOf('/') + 1;
            int endIndex = scenePath.LastIndexOf('.');

            return scenePath.Substring(startIndex, endIndex - startIndex);
        }

        private static void OpenScene(string scenePath)
        {
            EditorSceneManager.OpenScene(scenePath);
        }
        
        private static void AddScene(string scenePath)
        {
            EditorSceneManager.OpenScene(scenePath,OpenSceneMode.Additive);
        }
    }
}