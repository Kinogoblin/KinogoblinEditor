using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Kinogoblin
{

    public class ChangeMaterial : EditorWindow
    {
        public static UnityEngine.Object source = null;

        private static Material checkedMaterial = null;

        public static void ChangeMaterialGUI()
        {
            GUILayout.Label("Base settings for new material", EditorStyles.boldLabel);
            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            source = EditorGUILayout.ObjectField("Set material", source, typeof(UnityEngine.Object), true);
            EditorGUILayout.EndHorizontal();
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
                SetMaterialButton.SetCheckMaterial(checkedMaterial);
            }

            GUILayout.Space(10f);

            if (GUILayout.Button("Set new material"))
            {
                Helpful.Debug("Kinogoblin Editor ", "Set new material");
                SetMaterialButton.SetMaterialNew();
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

            public static void SetCheckMaterial(Material mat)
            {
                if (Selection.activeGameObject != null)
                {
                    CheckCheckedMaterial(Selection.activeGameObject, mat);
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

            static void CheckCheckedMaterial(GameObject activeObject, Material mat)
            {
                var AllObjects = new List<Transform>();
                AllObjects.Add(activeObject.transform);
                Helpful.GetListOfAllChilds(activeObject.transform, AllObjects);
                if (mat != null)
                {
                    ChangeCheckedMaterial(AllObjects, activeObject, mat);
                }
                else
                {
                    ChangeCheckedMaterial(AllObjects, activeObject);
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

            static void ChangeCheckedMaterial(List<Transform> list, GameObject go)
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
                                item.GetComponent<MeshRenderer>().material = mat;
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
            static void ChangeCheckedMaterial(List<Transform> list, GameObject go, Material mat)
            {
                foreach (var item in list)
                {
                    if (item.GetComponent<MeshRenderer>() != null)
                    {
                        item.GetComponent<MeshRenderer>().material = mat;
                    }
                }
            }
        }
    }
}