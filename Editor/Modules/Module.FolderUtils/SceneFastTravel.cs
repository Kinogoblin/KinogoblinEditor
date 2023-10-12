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
        private static List<string> scenePaths = new();
        private static Vector2 scrollPosition;
        
        public static void SceneFastTravelGUI()
        {
            if (EditorBuildSettings.scenes.Length != scenePaths.Count)
            {
                GetScenePaths();
            }
            GUILayout.Label("Scenes in Build Settings", EditorStyles.boldLabel);

            // scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < scenePaths.Count; i++)
            {
                if (GUILayout.Button(GetSceneNameFromPath(scenePaths[i]), GUILayout.Height(20)))
                {
                    OpenScene(scenePaths[i]);
                }
            }
            // EditorGUILayout.EndScrollView();
        }

        private static void GetScenePaths()
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            scenePaths = new List<string>();

            for (int i = 0; i < scenes.Length; i++)
            {
                scenePaths.Add(scenes[i].path);
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
    }
}