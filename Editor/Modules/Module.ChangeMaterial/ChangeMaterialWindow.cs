using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Kinogoblin.Editor.FavoriteAssets;
using Kinogoblin.Runtime;

namespace Kinogoblin.Editor
{
    public static class ChangeMaterialWindow
    {
        private static Material _selectedMaterial;
        private static bool _includeSkinnedMesh = false;
        private static string _materialsPath;

        private static GUIStyle _headerStyle;

        public static void ChangeMaterialOnGUI()
        {
            _materialsPath ??= ProfileData.Instance.pathForMaterials;
            if (_headerStyle == null)
            {
                _headerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 14
                };
            }

            GUILayout.Space(5);
            GUILayout.Label("⚙ Change Materials Tool", _headerStyle);
            GUILayout.Space(10);

            EditorGUILayout.LabelField("Selected Material", EditorStyles.boldLabel);
            _selectedMaterial = (Material)EditorGUILayout.ObjectField(_selectedMaterial, typeof(Material), false);

            _includeSkinnedMesh = EditorGUILayout.Toggle("Include Skinned Mesh Renderer", _includeSkinnedMesh);

            GUILayout.Space(10);

            if (GUILayout.Button("Apply Material to Selected Object(s)", GUILayout.Height(30)))
            {
                ApplyMaterialToSelection();
            }

            GUILayout.Space(10);
            GUILayout.Label("Material Save Settings", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            _materialsPath = EditorGUILayout.TextField("Save Path", _materialsPath);
            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                string newPath = EditorUtility.SaveFolderPanel("Select Material Save Path", _materialsPath, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    if (newPath.Contains("Assets"))
                    {
                        _materialsPath = "Assets" + newPath.Substring(newPath.IndexOf("Assets") + 6);
                        ProfileData.Instance.pathForMaterials = _materialsPath;
                    }
                    else
                    {
                        Debug.LogError("<color=cyan>Kinogoblin Editor</color> Path must be inside the project!");
                    }
                }
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            if (GUILayout.Button("Save Materials From Selected Object", GUILayout.Height(30)))
            {
                SaveMaterialsFromSelection();
            }
        }

        private static void ApplyMaterialToSelection()
        {
            if (!_selectedMaterial)
            {
                Debug.LogError("<color=cyan>Kinogoblin Editor</color> Please select a material first!");
                return;
            }

            if (Selection.transforms.Length == 0)
            {
                Debug.LogError("<color=cyan>Kinogoblin Editor</color> Please select at least one object in the scene!");
                return;
            }

            foreach (var transform in Selection.transforms)
            {
                MaterialManager.ApplyMaterialToHierarchy(transform, _selectedMaterial, _includeSkinnedMesh);
            }

            Debug.Log("<color=cyan>Kinogoblin Editor</color> ✅ Materials applied successfully!");
        }

        private static void SaveMaterialsFromSelection()
        {
            if (!Selection.activeTransform)
            {
                Debug.LogError("<color=cyan>Kinogoblin Editor</color> Please select an object in the scene!");
                return;
            }

            if (!Directory.Exists(_materialsPath))
                Directory.CreateDirectory(_materialsPath);

            MaterialSaver.SaveMaterials(Selection.activeTransform, _materialsPath);
            Debug.Log("<color=cyan>Kinogoblin Editor</color> ✅ Materials saved to: " + _materialsPath);
        }
    }
}