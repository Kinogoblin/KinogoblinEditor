﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.AI;
using System.Globalization;
using UnityEditor;
using System.IO;

namespace Kinogoblin
{

    public class SettingsForGameobject : EditorWindow
    {
        private static string GENERATED_COLLIDER_NAME = "__GeneratedCollider";
        private static string GENERATED_NAVMESH_OBSTACLE_NAME = "__GeneratedNavMeshObstacle";

        private static string UNDO_CREATE_PIVOT_REFERENCE = "Create Pivot Reference";
        private static string UNDO_ADJUST_PIVOT = "Move Pivot";
        private static string UNDO_SAVE_MODEL_AS = "Save Model As";

        private static bool createColliderObjectOnPivotChange = false;
        private static bool createNavMeshObstacleObjectOnPivotChange = false;

        private static readonly GUILayoutOption buttonHeight = GUILayout.Height(30);
        private static readonly GUILayoutOption headerHeight = GUILayout.Height(25);

        private static GUIStyle buttonStyle;
        private static GUIStyle headerStyle;

        private static Vector3 selectionPrevPos;
        private static Vector3 selectionPrevRot;

        private void OnEnable()
        {
            GetPrefs();

            Selection.selectionChanged += Repaint;
            EditorApplication.update += OnUpdate;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= Repaint;
            EditorApplication.update -= OnUpdate;
        }

        private void OnUpdate()
        {
            Transform selection = Selection.activeTransform;
            if (!IsNull(selection))
            {
                Vector3 pos = selection.localPosition;
                Vector3 rot = selection.localEulerAngles;

                if (pos != selectionPrevPos || rot != selectionPrevRot)
                {
                    Repaint();

                    selectionPrevPos = pos;
                    selectionPrevRot = rot;
                }
            }
        }

        private static void GetPrefs()
        {
            createColliderObjectOnPivotChange = EditorPrefs.GetBool("AdjustPivotCreateColliders", false);
            createNavMeshObstacleObjectOnPivotChange = EditorPrefs.GetBool("AdjustPivotCreateNavMeshObstacle", false);
        }

        private static void SetParentPivot(Transform pivot)
        {
            Transform pivotParent = pivot.parent;
            if (IsPrefab(pivotParent))
            {
                Debug.LogWarning("Modifying prefabs directly is not allowed, create an instance in the scene instead!");
                return;
            }

            if (pivot.localPosition == Vector3.zero && pivot.localEulerAngles == Vector3.zero)
            {
                Debug.LogWarning("Pivot hasn't changed!");
                return;
            }

            MeshFilter meshFilter = pivotParent.GetComponent<MeshFilter>();
            Mesh originalMesh = null;
            if (!IsNull(meshFilter) && !IsNull(meshFilter.sharedMesh))
            {
                Undo.RecordObject(meshFilter, UNDO_ADJUST_PIVOT);

                originalMesh = meshFilter.sharedMesh;
                Mesh mesh = Instantiate(meshFilter.sharedMesh);
                meshFilter.sharedMesh = mesh;

                Vector3[] vertices = mesh.vertices;
                Vector3[] normals = mesh.normals;
                Vector4[] tangents = mesh.tangents;

                if (pivot.localPosition != Vector3.zero)
                {
                    Vector3 deltaPosition = -pivot.localPosition;
                    for (int i = 0; i < vertices.Length; i++)
                        vertices[i] += deltaPosition;
                }

                if (pivot.localEulerAngles != Vector3.zero)
                {
                    Quaternion deltaRotation = Quaternion.Inverse(pivot.localRotation);
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        vertices[i] = deltaRotation * vertices[i];
                        normals[i] = deltaRotation * normals[i];

                        Vector3 tangentDir = deltaRotation * tangents[i];
                        tangents[i] = new Vector4(tangentDir.x, tangentDir.y, tangentDir.z, tangents[i].w);
                    }
                }

                mesh.vertices = vertices;
                mesh.normals = normals;
                mesh.tangents = tangents;

                mesh.RecalculateBounds();
            }

            GetPrefs();

            Collider[] colliders = pivotParent.GetComponents<Collider>();
            foreach (Collider collider in colliders)
            {
                MeshCollider meshCollider = collider as MeshCollider;
                if (!IsNull(meshCollider) && !IsNull(originalMesh) && meshCollider.sharedMesh == originalMesh)
                {
                    Undo.RecordObject(meshCollider, UNDO_ADJUST_PIVOT);
                    meshCollider.sharedMesh = meshFilter.sharedMesh;
                }
            }

            if (createColliderObjectOnPivotChange && IsNull(pivotParent.Find(GENERATED_COLLIDER_NAME)))
            {
                GameObject colliderObj = null;
                foreach (Collider collider in colliders)
                {
                    if (IsNull(collider))
                        continue;

                    MeshCollider meshCollider = collider as MeshCollider;
                    if (IsNull(meshCollider) || meshCollider.sharedMesh != meshFilter.sharedMesh)
                    {
                        if (colliderObj == null)
                        {
                            colliderObj = new GameObject(GENERATED_COLLIDER_NAME);
                            colliderObj.transform.SetParent(pivotParent, false);
                        }

                        EditorUtility.CopySerialized(collider, colliderObj.AddComponent(collider.GetType()));
                    }
                }

                if (colliderObj != null)
                    Undo.RegisterCreatedObjectUndo(colliderObj, UNDO_ADJUST_PIVOT);
            }

            if (createNavMeshObstacleObjectOnPivotChange && IsNull(pivotParent.Find(GENERATED_NAVMESH_OBSTACLE_NAME)))
            {
                NavMeshObstacle obstacle = pivotParent.GetComponent<NavMeshObstacle>();
                if (!IsNull(obstacle))
                {
                    GameObject obstacleObj = new GameObject(GENERATED_NAVMESH_OBSTACLE_NAME);
                    obstacleObj.transform.SetParent(pivotParent, false);
                    EditorUtility.CopySerialized(obstacle, obstacleObj.AddComponent(obstacle.GetType()));
                    Undo.RegisterCreatedObjectUndo(obstacleObj, UNDO_ADJUST_PIVOT);
                }
            }

            Transform[] children = new Transform[pivotParent.childCount];
            Vector3[] childrenPositions = new Vector3[children.Length];
            Quaternion[] childrenRotations = new Quaternion[children.Length];
            for (int i = children.Length - 1; i >= 0; i--)
            {
                children[i] = pivotParent.GetChild(i);
                childrenPositions[i] = children[i].position;
                childrenRotations[i] = children[i].rotation;

                Undo.RecordObject(children[i], UNDO_ADJUST_PIVOT);
            }

            Undo.RecordObject(pivotParent, UNDO_ADJUST_PIVOT);
            pivotParent.position = pivot.position;
            pivotParent.rotation = pivot.rotation;

            for (int i = 0; i < children.Length; i++)
            {
                children[i].position = childrenPositions[i];
                children[i].rotation = childrenRotations[i];
            }

            pivot.localPosition = Vector3.zero;
            pivot.localRotation = Quaternion.identity;
        }

        private static void SaveMesh(MeshFilter meshFilter,string name, bool saveAsAsset)
        {
            if (IsPrefab(meshFilter))
            {
                Debug.LogWarning("Modifying prefabs directly is not allowed, create an instance in the scene instead!");
                return;
            }

            string savedMeshName = meshFilter.sharedMesh.name;
            while (savedMeshName.EndsWith("(Clone)"))
                savedMeshName = savedMeshName.Substring(0, savedMeshName.Length - 7);

            string savePath = pathCustom + name +"_"+ savedMeshName + "." + (saveAsAsset ? "asset" : "obj");
            if (string.IsNullOrEmpty(savePath))
                return;
            Helpful.Debug(savePath);

            Mesh originalMesh = meshFilter.sharedMesh;
            Mesh savedMesh = saveAsAsset ? SaveMeshAsAsset(meshFilter, savePath) : SaveMeshAsOBJ(meshFilter, savePath);
            if (meshFilter.sharedMesh != savedMesh)
            {
                Undo.RecordObject(meshFilter, UNDO_SAVE_MODEL_AS);
                meshFilter.sharedMesh = savedMesh;
            }

            MeshCollider[] meshColliders = meshFilter.GetComponents<MeshCollider>();
            foreach (MeshCollider meshCollider in meshColliders)
            {
                if (!IsNull(meshCollider) && meshCollider.sharedMesh == originalMesh && meshCollider.sharedMesh != savedMesh)
                {
                    Undo.RecordObject(meshCollider, UNDO_SAVE_MODEL_AS);
                    meshCollider.sharedMesh = savedMesh;
                }
            }
        }

        private static Mesh SaveMeshAsAsset(MeshFilter meshFilter, string savePath)
        {
            Mesh mesh = meshFilter.sharedMesh;
            if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(mesh))) // If mesh is an asset, clone it
                mesh = Instantiate(mesh);

            AssetDatabase.CreateAsset(mesh, savePath);
            AssetDatabase.SaveAssets();

            return mesh;
        }


        private static Mesh SaveMeshAsOBJ(MeshFilter meshFilter, string savePath)
        {
            Mesh mesh = meshFilter.sharedMesh;

            Renderer renderer = meshFilter.GetComponent<Renderer>();
            Material[] mats = !IsNull(renderer) ? renderer.sharedMaterials : null;

            StringBuilder meshString = new StringBuilder();

            meshString.Append("g ").Append(Path.GetFileNameWithoutExtension(savePath)).Append("\n");
            foreach (Vector3 v in mesh.vertices)
                meshString.Append(string.Format(CultureInfo.InvariantCulture, "v {0} {1} {2}\n", -v.x, v.y, v.z));

            meshString.Append("\n");

            foreach (Vector3 v in mesh.normals)
                meshString.Append(string.Format(CultureInfo.InvariantCulture, "vn {0} {1} {2}\n", -v.x, v.y, v.z));

            meshString.Append("\n");

            foreach (Vector3 v in mesh.uv)
                meshString.Append(string.Format(CultureInfo.InvariantCulture, "vt {0} {1}\n", v.x, v.y));

            for (int material = 0; material < mesh.subMeshCount; material++)
            {
                meshString.Append("\n");

                if (mats != null && mats.Length > material)
                {
                    meshString.Append("usemtl ").Append(mats[material].name).Append("\n");
                    meshString.Append("usemap ").Append(mats[material].name).Append("\n");
                }

                int[] triangles = mesh.GetTriangles(material);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    meshString.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n",
                        triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
                }
            }

            File.WriteAllText(savePath, meshString.ToString());
            AssetDatabase.ImportAsset(savePath, ImportAssetOptions.ForceUpdate);

            return AssetDatabase.LoadAssetAtPath<Mesh>(savePath);
        }

        private static bool IsPrefab(Object obj)
        {
            return AssetDatabase.Contains(obj);
        }

        private static bool IsNull(Object obj)
        {
            return obj == null || obj.Equals(null);
        }

        /// //////////////////////
        /// 

        static string pathCustom = "Assets/__Project__/Models/MeshAssets/";

        public static void SettingsForGameobjectGUI()
        {

            GUILayout.Box("SETTINGS FOR GO", headerStyle, GUILayout.ExpandWidth(true), headerHeight);

            GUILayout.Space(10f);

            if (GUILayout.Button("Create child Pivote in center"))
            {
                SetGameObjestSettings.SetPivote();
            }
            if (GUILayout.Button("Create group of GO"))
            {
                SetGameObjestSettings.CreateGroup();
            }

            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle(GUI.skin.button) { richText = true };
                headerStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter };
            }


            GUILayout.Box("ADJUST PIVOT", headerStyle, GUILayout.ExpandWidth(true), headerHeight);

            Transform selection = Selection.activeTransform;
            if (!IsNull(selection))
            {
                if (!IsNull(selection.parent))
                {
                    if (selection.localPosition != Vector3.zero || selection.localEulerAngles != Vector3.zero)
                    {
                        if (GUILayout.Button("Move <b>" + selection.parent.name + "</b>'s pivot here", buttonStyle, buttonHeight))
                            SetParentPivot(selection);

                        if (selection.localEulerAngles != Vector3.zero)
                            EditorGUILayout.HelpBox("Pivot will also be rotated to match " + selection.name + "'s rotation.", MessageType.None);
                    }
                    else
                    {
                        GUI.enabled = false;
                        GUILayout.Button("Selected object is at pivot position", buttonStyle, buttonHeight);
                        GUI.enabled = true;
                    }
                }
                else
                {
                    GUI.enabled = false;
                    GUILayout.Button("Selected object has no parent", buttonStyle, buttonHeight);
                    GUI.enabled = true;
                }
            }
            else
            {
                GUI.enabled = false;
                GUILayout.Button("Nothing is selected", buttonStyle, buttonHeight);
                GUI.enabled = true;
            }

            GUILayout.Space(15f);

            GUILayout.Box("MESH UTILITY", headerStyle, GUILayout.ExpandWidth(true), headerHeight);

            GUILayout.Space(5f);

            GUILayout.Label("Custom path " + pathCustom);

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
                            pathCustom += tempPath[j]+"/";
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

            EditorGUILayout.HelpBox("If an object has a MeshFilter, changing its pivot will modify the mesh. That modified mesh must be saved before it can be applied to prefab.", MessageType.None);

            if (!IsNull(selection))
            {
                MeshFilter meshFilter = selection.GetComponent<MeshFilter>();
                if (!IsNull(meshFilter) && !IsNull(meshFilter.sharedMesh))
                {
                    if (GUILayout.Button("Save <b>" + selection.name + "</b>'s mesh as Asset (Recommended)", buttonStyle, buttonHeight))
                    {
                        if (!Directory.Exists(pathCustom))
                        {
                            Directory.CreateDirectory(pathCustom);
                        }
                        SaveMesh(meshFilter, selection.name, true);
                    }

                    GUILayout.Space(5f);

                    if (GUILayout.Button("Save <b>" + selection.name + "</b>'s mesh as OBJ", buttonStyle, buttonHeight))
                    {
                        if (!Directory.Exists(pathCustom))
                        {
                            Directory.CreateDirectory(pathCustom);
                        }
                        SaveMesh(meshFilter, selection.name, false);
                    }
                }
                else
                {
                    GUI.enabled = false;
                    GUILayout.Button("Selected object has no mesh", buttonStyle, buttonHeight);
                    GUI.enabled = true;
                }

                GUILayout.Space(5f);

                if (GUILayout.Button("Save all meshes as Asset (Recommended)", buttonStyle, buttonHeight))
                {
                    if (!Directory.Exists(pathCustom))
                    {
                        Directory.CreateDirectory(pathCustom);
                    }
                    for (int i = 0; i < Selection.gameObjects.Length; i++)
                    {
                        MeshFilter meshFilterGO = Selection.gameObjects[i].GetComponent<MeshFilter>();
                        if (!IsNull(meshFilterGO) && !IsNull(meshFilterGO.sharedMesh))
                        {
                            SaveMesh(meshFilterGO, Selection.gameObjects[i].name+""+i, true);
                        }
                    }
                }

                GUILayout.Space(5f);

                if (GUILayout.Button("Save all meshes as OBJ", buttonStyle, buttonHeight))
                {
                    if (!Directory.Exists(pathCustom))
                    {
                        Directory.CreateDirectory(pathCustom);
                    }
                    for (int i = 0; i < Selection.gameObjects.Length; i++)
                    {
                        MeshFilter meshFilterGO = Selection.gameObjects[i].GetComponent<MeshFilter>();
                        if (!IsNull(meshFilterGO) && !IsNull(meshFilterGO.sharedMesh))
                        {
                            SaveMesh(meshFilterGO, Selection.gameObjects[i].name + "" + i, false);
                        }
                    }
                }
            }
            else
            {
                GUI.enabled = false;
                GUILayout.Button("Nothing is selected", buttonStyle, buttonHeight);
                GUI.enabled = true;
            }

            GUILayout.Space(15f);

            GUILayout.Box("SETTINGS", headerStyle, GUILayout.ExpandWidth(true), headerHeight);

            EditorGUI.BeginChangeCheck();
            createColliderObjectOnPivotChange = EditorGUILayout.ToggleLeft("Create Child Collider Object On Pivot Change", createColliderObjectOnPivotChange);
            EditorGUILayout.HelpBox("Note that original collider(s) (if exists) will not be destroyed automatically.", MessageType.None);
            if (EditorGUI.EndChangeCheck())
                EditorPrefs.SetBool("AdjustPivotCreateColliders", createColliderObjectOnPivotChange);

            GUILayout.Space(10f);

            EditorGUI.BeginChangeCheck();
            createNavMeshObstacleObjectOnPivotChange = EditorGUILayout.ToggleLeft("Create Child NavMesh Obstacle Object On Pivot Change", createNavMeshObstacleObjectOnPivotChange);
            EditorGUILayout.HelpBox("Note that original NavMesh Obstacle (if exists) will not be destroyed automatically.", MessageType.None);
            if (EditorGUI.EndChangeCheck())
                EditorPrefs.SetBool("AdjustPivotCreateNavMeshObstacle", createNavMeshObstacleObjectOnPivotChange);

            GUILayout.Space(10f);

            if (GUILayout.Button("Tern off rendering shadows for gameObjects"))
            {
                Debug.Log("<color=blue>Kinogoblin Editor</color> Off shadows");
                SetGameObjestSettings.Shadows(false);
            }
            if (GUILayout.Button("Tern on rendering shadows for gameObjects"))
            {
                Debug.Log("<color=blue>Kinogoblin Editor</color> On shadows");
                SetGameObjestSettings.Shadows(true);
            }
        }

        static class SetGameObjestSettings
        {

            [MenuItem("Tools/Kinogoblin tools/Shortcuts/NewPivote #p")]
            static public void SetPivote()
            {
                if (Selection.activeGameObject != null)
                {
                    Debug.Log("<color=blue>Kinogoblin Editor</color> Set new pivote for " + Selection.activeGameObject.name);
                    CreatePivote(Selection.activeGameObject);
                }
                else
                {
                    Debug.Log("<color=blue>Kinogoblin Editor</color> Please select the want object in the scene");
                }
            }

            static public void Shadows(bool on)
            {
                if (Selection.activeGameObject != null)
                {
                    Debug.Log("<color=blue>Kinogoblin Editor</color> Set shadows on " + Selection.activeGameObject.name);
                    var AllObjects = new List<Transform>();
                    AllObjects.Add(Selection.activeGameObject.transform);
                    GetListOfAllChilds(Selection.activeGameObject.transform, AllObjects);
                    foreach (var item in AllObjects)
                    {
                        ShadowsEnable(on, item.gameObject);
                    }
                }
                else
                {
                    Debug.Log("<color=blue>Kinogoblin Editor</color> Please select the want object in the scene");
                }
            }

            static void ShadowsEnable(bool on, GameObject go)
            {
                if (go.GetComponent<MeshRenderer>() != null)
                {
                    if (on)
                    {
                        go.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    }
                    else
                    {
                        go.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }
                }
            }


            static void CheckGameObject(GameObject activeObject)
            {
                var AllObjects = new List<Transform>();
                AllObjects.Add(activeObject.transform);
                GetListOfAllChilds(activeObject.transform, AllObjects);
            }


            static void GetListOfAllChilds(Transform parent, List<Transform> list)
            {
                foreach (Transform child in parent)
                {
                    list.Add(child);
                    GetListOfAllChilds(child, list);
                }
            }

            static void CreatePivote(GameObject go)
            {
                if (go.GetComponent<Renderer>() != null)
                {
                    GameObject pivote = new GameObject();
                    pivote.name = go.name + "_pivote";
                    pivote.transform.position = go.GetComponent<Renderer>().bounds.center;
                    if (go.transform.parent != null)
                    {
                        pivote.transform.parent = go.transform;
                    }
                }
                else
                {
                    Debug.Log("<color=blue>Kinogoblin Editor</color> GameObject haven't component Renderer");
                }
            }


            [MenuItem("Tools/Kinogoblin tools/Shortcuts/Create Group #g", false, 4)]
            public static void CreateGroup()
            {
                GameObject newGO = new GameObject("Group");
                foreach (GameObject go in Selection.gameObjects)
                    go.transform.parent = newGO.transform;
                Selection.activeGameObject = newGO;
                CenterOnChildren();
            }

            [MenuItem("Tools/Kinogoblin tools/Shortcuts/Center Group on Children", false, 5)]
            public static void CenterOnChildren()
            {
                foreach (Transform root in Selection.GetFiltered(typeof(Transform), SelectionMode.TopLevel | SelectionMode.ExcludePrefab | SelectionMode.Editable))
                {
                    Vector3 min = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
                    Vector3 max = new Vector3(-Mathf.Infinity, -Mathf.Infinity, -Mathf.Infinity);
                    List<Vector3> origPos = new List<Vector3>();
                    bool found = false;

                    foreach (Transform t in root)
                    {
                        found = true;
                        min = Vector3.Min(min, t.position);
                        max = Vector3.Max(max, t.position);
                        origPos.Add(t.position);
                    }

                    if (found)
                    {
                        Vector3 centerPoint = (max + min) / 2f;
                        root.position = centerPoint;

                        int idx = 0;
                        foreach (Transform t in root)
                            t.position = origPos[idx++];
                    }
                }
            }

        }
    }

}