using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace Kinogoblin.Editor
{
    public static class MaterialSaver
    {
        public static void SaveMaterials(Transform root, string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var renderers = root.GetComponentsInChildren<Renderer>(true);
            var savedMaterials = new List<Material>();
            
            foreach (var renderer in renderers)
            {
                Undo.RecordObject(renderer, "Save Materials");

                var newMaterials = new List<Material>();

                foreach (var mat in renderer.sharedMaterials)
                {
                    var materialPath = Path.Combine(path, $"{mat.name}.mat");

                    if (!AssetDatabase.LoadAssetAtPath<Material>(materialPath))
                    {
                        var newMat = new Material(mat.shader);
                        newMat.CopyPropertiesFromMaterial(mat);
                        AssetDatabase.CreateAsset(newMat, materialPath);
                    }

                    var savedMat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                    newMaterials.Add(savedMat);
                    savedMaterials.Add(savedMat);
                }

                renderer.sharedMaterials = newMaterials.ToArray();
                EditorUtility.SetDirty(renderer);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            if (savedMaterials.Count == 1)
            {
                Selection.activeObject = savedMaterials[0];
                EditorGUIUtility.PingObject(savedMaterials[0]);
            }
            else if (savedMaterials.Count > 1)
            {
                var folderObj = AssetDatabase.LoadAssetAtPath<Object>(path);
                Selection.activeObject = folderObj;
                EditorGUIUtility.PingObject(folderObj);
            }
        }
    }
}