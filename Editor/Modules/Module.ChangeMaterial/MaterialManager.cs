using UnityEngine;
using UnityEditor;
using System.IO;

namespace Kinogoblin.Editor
{
    public static class MaterialManager
    {
        public static void ApplyMaterialToHierarchy(Transform root, Material material, bool includeSkinnedMesh)
        {
            var renderers = root.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                Undo.RecordObject(renderer, "Change Material");
                var mats = renderer.sharedMaterials;
                for (int i = 0; i < mats.Length; i++)
                    mats[i] = material;
                renderer.sharedMaterials = mats;
                EditorUtility.SetDirty(renderer);
            }

            if (includeSkinnedMesh)
            {
                var skinnedRenderers = root.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                foreach (var skinned in skinnedRenderers)
                {
                    Undo.RecordObject(skinned, "Change Material");
                    var mats = skinned.sharedMaterials;
                    for (int i = 0; i < mats.Length; i++)
                        mats[i] = material;
                    skinned.sharedMaterials = mats;
                    EditorUtility.SetDirty(skinned);
                }
            }
        }

        public static Material CreateMaterialAsset(string path, string name, Shader shader)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var materialPath = Path.Combine(path, $"{name}.mat");
            var material = new Material(shader);
            AssetDatabase.CreateAsset(material, materialPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return material;
        }
    }
}