using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Kinogoblin.Editor.FavoriteAssets;

namespace Kinogoblin.Editor
{

    public class ChangeMaterial : EditorWindow
    {
        public static UnityEngine.Object source = null;

        private static Material checkedMaterial = null;

        private static GUIStyle buttonStyle;
        private static GUIStyle headerStyle;
        private static readonly GUILayoutOption buttonHeight = GUILayout.Height(30);
        private static readonly GUILayoutOption headerHeight = GUILayout.Height(25);

        private static bool _updateSkinnedMeshRenderer = false;

        public static string pathCustom
        {
            get
            {
                return ProfileData.Instance.pathForMaterials;
            }
            set
            {
                ProfileData.Instance.pathForMaterials = value;
            }
        }

        public static void ChangeMaterialGUI()
        {
            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle(GUI.skin.button) { richText = true };
                headerStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
            }

            GUILayout.Box("SETTINGS FOR GO", headerStyle, GUILayout.ExpandWidth(true), headerHeight);

            GUILayout.Label("Base settings for new material", EditorStyles.boldLabel);
            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            source = EditorGUILayout.ObjectField("Set material", source, typeof(UnityEngine.Object), true);
            EditorGUILayout.EndHorizontal();

            _updateSkinnedMeshRenderer = GUILayout.Toggle(_updateSkinnedMeshRenderer, "Update material in SkinnedMeshRenderer");

            if (GUILayout.Button("Set checked material"))
            {
                Helpful.Debug("Kinogoblin Editor ", "Set checked material");
                if (source != null)
                {
                    try
                    {
                        checkedMaterial = (Material)source;
                    }
                    catch
                    {
                        checkedMaterial = null;
                        Debug.LogError("<color=blue>Kinogoblin Editor</color> This file is not material! I'll look for material by object name");
                    }
                }
                SetMaterialButton.SetCheckMaterial(checkedMaterial, _updateSkinnedMeshRenderer);
            }

            GUILayout.Space(10f);

            if (GUILayout.Button("Set new material"))
            {
                Helpful.Debug("Kinogoblin Editor ", "Set new material");
                SetMaterialButton.SetMaterialNew();
            }

            GUILayout.Box("MATERIAL SAVER FROM OBJECTS", headerStyle, GUILayout.ExpandWidth(true), headerHeight);

            GUILayout.Space(5f);

            GUILayout.Label("Custom path " + pathCustom);

            if (GUILayout.Button("Set default path", buttonStyle, buttonHeight))
            {
                pathCustom = "Assets/__Project__/Materials/";
                Helpful.Debug(pathCustom);
            }
            if (GUILayout.Button("Save custom path", buttonStyle, buttonHeight))
            {
                var temp = pathCustom;
                pathCustom = EditorUtility.SaveFolderPanel("Save custom path", "", string.Empty);
                int i = 0;
                bool normalPath = false;
                string[] tempPath = Helpful.GetName(pathCustom, '/');
                foreach (var item in tempPath)
                {
                    if (item.Contains("Assets"))
                    {
                        normalPath = true;
                        pathCustom = "";
                        for (int j = i; j < tempPath.Length; j++)
                        {
                            pathCustom += tempPath[j] + "/";
                        }
                        continue;
                    }
                    else
                    {
                        i++;
                    }
                }
                if (!normalPath)
                {
                    Helpful.Debug("Find path in project!!!");
                    pathCustom = temp;
                }
                else
                {
                    Helpful.Debug(pathCustom);
                }
            }

            Transform selection = Selection.activeTransform;

            if (!IsNull(selection))
            {
                if (GUILayout.Button("Copy and save <b>" + selection.name + "</b>'s materials!", buttonStyle, buttonHeight))
                {
                    var selectionName = selection.name;
                    selectionName = selectionName.Trim();
                    var pathWithName = pathCustom + "/" + selectionName + "/";
                    if (!Directory.Exists(pathWithName))
                    {
                        Directory.CreateDirectory(pathWithName);
                    }
                    SaveMaterialButton.SaveMaterialFromGameObject(pathWithName, selection);
                }

                GUILayout.Space(5f);
            }
            else
            {
                GUI.enabled = false;
                GUILayout.Button("Selected object for save materials", buttonStyle, buttonHeight);
                GUI.enabled = true;
            }

        }


        static class SetMaterialButton
        {
            public static void SetMaterialNew()
            {
                if (Selection.activeGameObject != null)
                    CheckNewMaterial(Selection.activeGameObject);
                else
                {
                    Helpful.Debug("Kinogoblin Editor ", "Please select the want object in the scene");
                }
            }

            public static void SetCheckMaterial(Material mat, bool updateSkinnedMeshRenderer)
            {
                if (Selection.activeGameObject != null)
                {
                    CheckCheckedMaterial(Selection.activeGameObject, mat, updateSkinnedMeshRenderer);
                }
                else
                {
                    Helpful.Debug("Kinogoblin Editor ", "Please select the want object in the scene");
                }
            }


            static void CheckNewMaterial(GameObject activeObject)
            {
                var AllObjects = new List<Transform>();
                AllObjects.Add(activeObject.transform);
                Helpful.GetListOfAllChilds(activeObject.transform, AllObjects);
                ChangeNewMaterial(AllObjects, activeObject);
            }

            static void CheckCheckedMaterial(GameObject activeObject, Material mat, bool updateSkinnedMeshRenderer)
            {
                var AllObjects = new List<Transform>();
                AllObjects.Add(activeObject.transform);
                Helpful.GetListOfAllChilds(activeObject.transform, AllObjects);
                if (mat != null)
                {
                    ChangeCheckedMaterial(AllObjects, activeObject, mat, updateSkinnedMeshRenderer);
                }
                else
                {
                    ChangeCheckedMaterial(AllObjects, activeObject, updateSkinnedMeshRenderer);
                }
            }


            static void ChangeNewMaterial(List<Transform> list, GameObject go)
            {
                Material material = new Material(Shader.Find("Standard"));
                AssetDatabase.CreateAsset(material, "Assets/" + go.name + ".mat");
                if (!Directory.Exists("Assets/Resources"))
                {
                    Directory.CreateDirectory("Assets/Resources");
                }
                File.Move("Assets/" + go.name + ".mat", "Assets/Resources/" + go.name + ".mat");
                AssetDatabase.Refresh();
                Material mat = Resources.Load(go.name) as Material;
                //if (!Directory.Exists("Assets/Materials"))
                //{
                //    Directory.CreateDirectory("Assets/Materials");
                //}
                //File.Move("Assets/Resources/" + go.name + ".mat", "Assets/Materials/" + go.name + ".mat");
                AssetDatabase.Refresh();
                foreach (var item in list)
                {
                    Debug.Log("ListIn");
                    if (item.GetComponent<MeshRenderer>() != null)
                    {
                        if (mat == null)
                            Debug.Log("MaterialIn");
                        item.GetComponent<MeshRenderer>().material = mat;
                    }
                }
            }

            static void ChangeCheckedMaterial(List<Transform> list, GameObject go, bool updateSkinnedMeshRenderer)
            {
                if (Directory.Exists("Assets/Resources/"))
                {
                    if (File.Exists("Assets/Resources/" + go.name + ".mat"))
                    {
                        Material mat = Resources.Load(go.name) as Material;
                        foreach (var item in list)
                        {
                            if (item.GetComponent<MeshRenderer>() != null)
                            {
                                var mats = item.GetComponent<MeshRenderer>().sharedMaterials;
                                for (int i = 0; i < mats.Length; i++)
                                {
                                    mats[i] = mat;
                                }
                                item.GetComponent<MeshRenderer>().sharedMaterials = mats;
                            }
                            if (updateSkinnedMeshRenderer)
                            {
                                var skinnedMeshRenderer = item.GetComponent<SkinnedMeshRenderer>();

                                if (skinnedMeshRenderer != null)
                                {
                                    var mats = skinnedMeshRenderer.sharedMaterials;
                                    for (int i = 0; i < mats.Length; i++)
                                    {
                                        mats[i] = mat;
                                    }
                                    skinnedMeshRenderer.sharedMaterials = mats;
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("<color=blue>Kinogoblin Editor</color> I couldn't find any material by object name!");
                    }
                }
                else
                {
                    Debug.LogError("<color=blue>Kinogoblin Editor</color> I couldn't find any material by object name!");
                }
            }

            static void ChangeCheckedMaterial(List<Transform> list, GameObject go, Material mat, bool updateSkinnedMeshRenderer)
            {
                foreach (var item in list)
                {
                    var meshRenderer = item.GetComponent<MeshRenderer>();
                    if (meshRenderer != null)
                    {
                        var mats = meshRenderer.materials;
                        for (int i = 0; i < mats.Length; i++)
                        {
                            mats[i] = mat;
                        }
                        meshRenderer.sharedMaterials = mats;
                    }
                    if (updateSkinnedMeshRenderer)
                    {
                        var skinnedMeshRenderer = item.GetComponent<SkinnedMeshRenderer>();

                        if (skinnedMeshRenderer != null)
                        {
                            var mats = skinnedMeshRenderer.sharedMaterials;
                            for (int i = 0; i < mats.Length; i++)
                            {
                                mats[i] = mat;
                            }
                            skinnedMeshRenderer.sharedMaterials = mats;
                        }
                    }
                }
            }
        }

        static class SaveMaterialButton
        {
            public static void SaveMaterialFromGameObject(string customPath, Transform activeTransform)
            {
                Debug.Log("Save Mat!");
                if (activeTransform.GetComponent<Renderer>() != null)
                {
                    Debug.Log("Save Mat! 1");
                    List<Material> new_materials = new List<Material>() { };
                    foreach (Material mat in activeTransform.GetComponent<Renderer>().sharedMaterials)
                    {
                        Material new_material = new Material(mat.shader);
                        new_material.CopyPropertiesFromMaterial(mat);
                        if (AssetDatabase.LoadAssetAtPath<Material>(customPath + mat.name + ".mat") == null)
                        {
                            AssetDatabase.CreateAsset(new_material, customPath + mat.name + ".mat");
                        }
                        new_materials.Add(AssetDatabase.LoadAssetAtPath<Material>(customPath + mat.name + ".mat"));
                    }
                    activeTransform.GetComponent<Renderer>().sharedMaterials = new_materials.ToArray();
                }
                SaveMaterialsInChilds(customPath, activeTransform);
            }

            public static void SaveMaterialsInChilds(string customPath, Transform activeTransform)
            {
                foreach (Transform child in activeTransform)
                {
                    if (child.GetComponent<Renderer>() != null)
                    {
                        Debug.Log("Save Mat! 2");
                        List<Material> new_materials = new List<Material>() { };
                        foreach (Material mat in child.GetComponent<Renderer>().sharedMaterials)
                        {
                            Material new_material = new Material(mat.shader);
                            new_material.CopyPropertiesFromMaterial(mat);
                            if (AssetDatabase.LoadAssetAtPath<Material>(customPath + mat.name + ".mat") == null)
                            {
                                AssetDatabase.CreateAsset(new_material, customPath + mat.name + ".mat");
                            }
                            new_materials.Add(AssetDatabase.LoadAssetAtPath<Material>(customPath + mat.name + ".mat"));
                        }
                        child.GetComponent<Renderer>().sharedMaterials = new_materials.ToArray();
                    }
                    SaveMaterialsInChilds(customPath, child);
                }

            }
        }

        private static bool IsNull(Object obj)
        {
            return obj == null || obj.Equals(null);
        }
    }
}