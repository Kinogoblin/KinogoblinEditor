namespace Kinogoblin
{
    //Version 1.9
    using System.Collections.Generic;
    using System.Globalization;
    using UnityEngine;
    using UnityEditor;
    using System.IO;
    using System.Text;
    using UnityEngine.AI;

    enum TypesOfSettings
    {
        FolderUtils,
        SettingsForGameobject,
        ChangeMaterial
    }

    class EditorSettings : EditorWindow
    {
        [MenuItem("Tools/Kinogoblin tools/Set Settings Window #w")]

        public static void ShowWindow()
        {
            GetWindow(typeof(EditorSettings));
        }

        private Material checkedMaterial = null;
        private TypesOfSettings type;

        private bool _smallSettings = false;
        public static Color hierarchyColor = new Color(0.5f,0,1);

        public UnityEngine.Object source = null;
        public GameObject gameObjectSource;

        void OnGUI()
        {
            GUILayout.Space(10f);

            type = (TypesOfSettings)EditorGUILayout.EnumPopup("Primitive to create:", type);

            GUILayout.Space(10f);

            switch (type)
            {
                case TypesOfSettings.ChangeMaterial:
                    ChangeMaterialGUI();
                    break;
                case TypesOfSettings.SettingsForGameobject:
                    SettingsForGameobjectGUI();
                    break;
                case TypesOfSettings.FolderUtils:
                    FolderUtilsGUI();
                    break;
                default:
                    ChangeMaterialGUI();
                    break;
            }
        }

        #region ChangeMaterial

        public void ChangeMaterialGUI()
        {
            GUILayout.Label("Base settings for new material", EditorStyles.boldLabel);
            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            source = EditorGUILayout.ObjectField("Set material", source, typeof(UnityEngine.Object), true);
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Set checked material"))
            {
                Debug.Log("<color=blue>Kinogoblin Editor</color> Set checked material");
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
                Debug.Log("<color=blue>Kinogoblin Editor</color> Set new material");

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
                    Debug.Log("<color=blue>Kinogoblin Editor</color> Please select the want object in the scene");
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
                    Debug.Log("<color=blue>Kinogoblin Editor</color> Please select the want object in the scene");
                }
            }


            static void CheckNewMaterial(GameObject activeObject)
            {
                var AllObjects = new List<Transform>();
                AllObjects.Add(activeObject.transform);
                GetListOfAllChilds(activeObject.transform, AllObjects);
                ChangeNewMaterial(AllObjects, activeObject);
            }

            static void CheckCheckedMaterial(GameObject activeObject, Material mat)
            {
                var AllObjects = new List<Transform>();
                AllObjects.Add(activeObject.transform);
                GetListOfAllChilds(activeObject.transform, AllObjects);
                if (mat != null)
                {
                    ChangeCheckedMaterial(AllObjects, activeObject, mat);
                }
                else
                {
                    ChangeCheckedMaterial(AllObjects, activeObject);
                }
            }

            static void GetListOfAllChilds(Transform parent, List<Transform> list)
            {
                foreach (Transform child in parent)
                {
                    list.Add(child);
                    GetListOfAllChilds(child, list);
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

            static string GetName(string name, char symbol, int numberName)
            {
                string newname = null;
                if (name.Contains(symbol.ToString()))
                    newname = name.Split(new char[] { symbol })[numberName];
                else
                    newname = name;
                return newname;
            }

            static string[] GetName(string name, char symbol)
            {
                string[] newname = null;
                if (name.Contains(symbol.ToString()))
                    newname = name.Split(new char[] { symbol });
                return newname;
            }

        }


        #endregion

        #region SettingsForGameobject

        /// //////////////////////

            #region CreateNewPivote

        private const string GENERATED_COLLIDER_NAME = "__GeneratedCollider";
        private const string GENERATED_NAVMESH_OBSTACLE_NAME = "__GeneratedNavMeshObstacle";

        private const string UNDO_CREATE_PIVOT_REFERENCE = "Create Pivot Reference";
        private const string UNDO_ADJUST_PIVOT = "Move Pivot";
        private const string UNDO_SAVE_MODEL_AS = "Save Model As";

        private bool createColliderObjectOnPivotChange = false;
        private bool createNavMeshObstacleObjectOnPivotChange = false;

        private readonly GUILayoutOption buttonHeight = GUILayout.Height(30);
        private readonly GUILayoutOption headerHeight = GUILayout.Height(25);

        private GUIStyle buttonStyle;
        private GUIStyle headerStyle;

        private Vector3 selectionPrevPos;
        private Vector3 selectionPrevRot;

        private Vector2 scrollPos = Vector2.zero;
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

        private void GetPrefs()
        {
            createColliderObjectOnPivotChange = EditorPrefs.GetBool("AdjustPivotCreateColliders", false);
            createNavMeshObstacleObjectOnPivotChange = EditorPrefs.GetBool("AdjustPivotCreateNavMeshObstacle", false);
        }

        private void SetParentPivot(Transform pivot)
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

        private void SaveMesh(MeshFilter meshFilter, bool saveAsAsset)
        {
            if (IsPrefab(meshFilter))
            {
                Debug.LogWarning("Modifying prefabs directly is not allowed, create an instance in the scene instead!");
                return;
            }

            string savedMeshName = meshFilter.sharedMesh.name;
            while (savedMeshName.EndsWith("(Clone)"))
                savedMeshName = savedMeshName.Substring(0, savedMeshName.Length - 7);

            string savePath = EditorUtility.SaveFilePanelInProject("Save As", savedMeshName, saveAsAsset ? "asset" : "obj", string.Empty);
            if (string.IsNullOrEmpty(savePath))
                return;

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

        private Mesh SaveMeshAsAsset(MeshFilter meshFilter, string savePath)
        {
            Mesh mesh = meshFilter.sharedMesh;
            if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(mesh))) // If mesh is an asset, clone it
                mesh = Instantiate(mesh);

            AssetDatabase.CreateAsset(mesh, savePath);
            AssetDatabase.SaveAssets();

            return mesh;
        }


        private Mesh SaveMeshAsOBJ(MeshFilter meshFilter, string savePath)
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

        private bool IsPrefab(Object obj)
        {
            return AssetDatabase.Contains(obj);
        }

        private bool IsNull(Object obj)
        {
            return obj == null || obj.Equals(null);
        }

        #endregion

        /// //////////////////////

        public void SettingsForGameobjectGUI()
        {
            GUILayout.Label("Settings for GameObjects", EditorStyles.boldLabel);

            GUILayout.Space(10f);

            if (GUILayout.Button("Create child Pivote in center"))
            {
                SetGameObjestSettings.SetPivote();
            }
            if (GUILayout.Button("Create group of GO"))
            {
                SetGameObjestSettings.GroupSelected();
            }

            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle(GUI.skin.button) { richText = true };
                headerStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter };
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

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

            EditorGUILayout.HelpBox("If an object has a MeshFilter, changing its pivot will modify the mesh. That modified mesh must be saved before it can be applied to prefab.", MessageType.None);

            if (!IsNull(selection))
            {
                MeshFilter meshFilter = selection.GetComponent<MeshFilter>();
                if (!IsNull(meshFilter) && !IsNull(meshFilter.sharedMesh))
                {
                    if (GUILayout.Button("Save <b>" + selection.name + "</b>'s mesh as Asset (Recommended)", buttonStyle, buttonHeight))
                        SaveMesh(meshFilter, true);

                    GUILayout.Space(5f);

                    if (GUILayout.Button("Save <b>" + selection.name + "</b>'s mesh as OBJ", buttonStyle, buttonHeight))
                        SaveMesh(meshFilter, false);
                }
                else
                {
                    GUI.enabled = false;
                    GUILayout.Button("Selected object has no mesh", buttonStyle, buttonHeight);
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

            EditorGUILayout.EndScrollView();
            //if (GUILayout.Button("Tern off rendering shadows for gameObjects"))
            //{
            //    Debug.Log("<color=blue>Kinogoblin Editor</color> Off shadows");
            //    SetGameObjestSettings.Shadows(false);
            //}
            //if (GUILayout.Button("Tern on rendering shadows for gameObjects"))
            //{
            //    Debug.Log("<color=blue>Kinogoblin Editor</color> On shadows");
            //    SetGameObjestSettings.Shadows(true);
            //}

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


            [MenuItem("Tools/Kinogoblin tools/Shortcuts/Group Selected #g")]
            static public void GroupSelected()
            {
                if (!Selection.activeTransform) return;
                var go = new GameObject(Selection.activeTransform.name + " Group");
                Undo.RegisterCreatedObjectUndo(go, "Group Selected");
                go.transform.SetParent(Selection.activeTransform.parent, false);
                Vector3 center = new Vector3();
                float x = 0;
                float y = 0;
                float z = 0;
                foreach (var transform in Selection.transforms)
                {
                    x += transform.position.x;
                    y += transform.position.y;
                    z += transform.position.z;
                }
                center = new Vector3((x/Selection.transforms.Length), (y / Selection.transforms.Length), (z / Selection.transforms.Length));
                go.transform.position = center;
                foreach (var transform in Selection.transforms)
                {
                    Undo.SetTransformParent(transform, go.transform, "Group Selected"); 
                }
                Selection.activeGameObject = go;
            }


        }


        #endregion

        #region FolderUtils

        public void FolderUtilsGUI()
        {
            GUILayout.Label("Scene utils", EditorStyles.boldLabel);
            GUILayout.Space(10f);
            if (GUILayout.Button("Create Scene Catalog"))
            {
                CreateProjectsComponents.SceneCreate();
            }
            hierarchyColor = EditorGUILayout.ColorField("Color divisions", hierarchyColor);
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
            Debug.Log("<color=blue>Kinogoblin Editor</color> Create scene catalog");
            var cameraObj = new GameObject("---Player---").transform;
            var scriptObj = new GameObject("---Managers---").transform;
            var lightObj = new GameObject("---Light---").transform;
            var staticObj = new GameObject("---Enviroment---").transform;
            var dinamicObj = new GameObject("---Interactable---").transform;
            var audioObj = new GameObject("---Sound---").transform;
            var timelines = new GameObject("---Timelines---").transform;
        }

        [MenuItem("Tools/Kinogoblin tools/Shortcuts/Create Folder Catalog #f")]
        public static void FolderCreate()
        {
            Debug.Log("<color=blue>Kinogoblin Editor</color> Create folder catalog");
            string path = "Assets / __Project__";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            Directory.CreateDirectory(path + "/Materials");
            Directory.CreateDirectory(path + "/Prefabs");
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
            Debug.Log("<color=blue>Kinogoblin Editor</color> Create folder catalog");
            string path = "Assets / __Project__";

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
            Debug.Log("<color=blue>Kinogoblin Editor</color> Create folder catalog");
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

    #endregion



#if UNITY_EDITOR
    public class EditorExtensions
    {
        [MenuItem("Assets/Add Folder to .gitignore")]
        static void AddFolderToGitignore()
        {
            var ignoreFile = Path.Combine(Application.dataPath, @"..\", ".gitignore");
            Debug.Log($"Constructed path: {ignoreFile}");

            if (!File.Exists(ignoreFile))
            {
                Debug.LogError($"[Editor Extenstions] .gitignore file not found. Operation aborted");
                return;
            }

            var selection = Selection.activeObject;

            if (selection == null) return;

            var selectionPath = AssetDatabase.GetAssetPath(selection.GetInstanceID());

            if (selectionPath.Length < 0 || !Directory.Exists(selectionPath))
            {
                Debug.LogError("[Editor Extensions] Selected object is not a folder. Operation aborted");
                return;
            }

            using (var writer = new StreamWriter(ignoreFile, true))
            {
                writer.WriteLine($"{selectionPath}/");
                writer.WriteLine($"{selectionPath}*.*meta");

                Debug.Log($"Folder {selection.name} successfully added to .gitignore");
            }
        }
    }

    [InitializeOnLoad]
    public static class HierarchyWindowGroupHeader
    {
        static HierarchyWindowGroupHeader()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }

        static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (gameObject != null && gameObject.name.StartsWith("---", System.StringComparison.Ordinal))
            {

                EditorGUI.DrawRect(selectionRect, EditorSettings.hierarchyColor);
                EditorGUI.DropShadowLabel(selectionRect, gameObject.name.Replace("-", "").ToUpperInvariant());
            }
        }
    }
#endif


}