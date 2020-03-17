namespace Kinogoblin
{
    //Version 1.7
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using System.IO;
    using UnityEditor.SceneManagement;
    using System;

    enum TypesOfSettings
    {
        ChangeMaterial,
        SettingsForGameobject,
        FolderUtils
    }

    class SetKinogoblinSettings : EditorWindow
    {
        [MenuItem("Tools/Kinogoblin tools/Set Settings Window")]

        public static void ShowWindow()
        {
            GetWindow(typeof(SetKinogoblinSettings));
        }

        private Material checkedMaterial = null;
        private TypesOfSettings type;

        private bool smallSettings = false;

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

        public void SettingsForGameobjectGUI()
        {
            GUILayout.Label("Settings for GameObjects", EditorStyles.boldLabel);

            GUILayout.Space(10f);

            if (GUILayout.Button("Create Pivote"))
            {
                SetGameObjestSettings.SetPivote(true);
            }
            if (GUILayout.Button("Create Pivote for several gameObjects"))
            {
                Debug.Log("<color=blue>Kinogoblin Editor</color> Set new material");

                SetGameObjestSettings.SetPivote(false);
            }
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

        public void FolderUtilsGUI()
        {
            GUILayout.Label("Folder utils", EditorStyles.boldLabel);
            GUILayout.Space(10f);
            if (GUILayout.Button("Create Scene Catalog"))
            {
                CreateProjectsComponents.SceneCreate();
            }
            if (GUILayout.Button("Create Folder Catalog"))
            {
                CreateProjectsComponents.FolderCreate();
            }
            smallSettings = GUILayout.Toggle(smallSettings, "Choose small variant");
            if (GUILayout.Button("Create Folder Catalog under active folder"))
            {
                CreateProjectsComponents.FolderCreateUnderActive(CreateProjectsComponents.GetSelectedPathOrFallback(), smallSettings);
            }
        }

        [MenuItem("Tools/Kinogoblin tools/Shortcuts/Group Selected #g")]
        private static void GroupSelected()
        {
            if (!Selection.activeTransform) return;
            var go = new GameObject(Selection.activeTransform.name + " Group");
            Undo.RegisterCreatedObjectUndo(go, "Group Selected");
            go.transform.SetParent(Selection.activeTransform.parent, false);
            foreach (var transform in Selection.transforms) Undo.SetTransformParent(transform, go.transform, "Group Selected");
            Selection.activeGameObject = go;
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
            var audioObj = new GameObject("---Audio---").transform;
            var lightObj = new GameObject("---Light---").transform;
            var staticObj = new GameObject("---Static Objects---").transform;
            var dinamicObj = new GameObject("---Dynamic Objects---").transform;
            var colliders = new GameObject("---Colliders---").transform;
            var timelines = new GameObject("---Timelines---").transform;
        }

        public static void FolderCreate()
        {
            Debug.Log("<color=blue>Kinogoblin Editor</color> Create folder catalog");
            if (!Directory.Exists("Assets/_Kinogoblin"))
            {
                Directory.CreateDirectory("Assets/_Kinogoblin");
            }
            Directory.CreateDirectory("Assets/_Kinogoblin/Animations");
            Directory.CreateDirectory("Assets/_Kinogoblin/Animations/Timelines");
            Directory.CreateDirectory("Assets/_Kinogoblin/Editor");
            Directory.CreateDirectory("Assets/_Kinogoblin/Animations/AnimationClips");
            Directory.CreateDirectory("Assets/_Kinogoblin/Audio");
            Directory.CreateDirectory("Assets/_Kinogoblin/Models");
            Directory.CreateDirectory("Assets/_Kinogoblin/Materials");
            Directory.CreateDirectory("Assets/_Kinogoblin/Prefabs");
            Directory.CreateDirectory("Assets/_Kinogoblin/Scripts");
            Directory.CreateDirectory("Assets/_Kinogoblin/Scripts/Managers");
            Directory.CreateDirectory("Assets/_Kinogoblin/Scenes");
            Directory.CreateDirectory("Assets/_Kinogoblin/Trash");
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

    static class SetGameObjestSettings
    {

        [MenuItem("Tools/Kinogoblin tools/Shortcuts/NewPivote #p")]
        static public void SetPivote(bool one)
        {
            if (Selection.activeGameObject != null)
            {
                if (one)
                {
                    Debug.Log("<color=blue>Kinogoblin Editor</color> Set new pivote for " + Selection.activeGameObject.name);
                    CreatePivote(Selection.activeGameObject);
                }
                else
                {
                    foreach (var item in Selection.gameObjects)
                    {
                        Debug.Log("<color=blue>Kinogoblin Editor</color> Set new pivote for " + item.name);
                        CreatePivote(item);
                    }
                }
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
                    pivote.transform.parent = go.transform.parent;
                }
                go.transform.parent = pivote.transform;
            }
            else
            {
                Debug.Log("<color=blue>Kinogoblin Editor</color> GameObject haven't component Renderer");
            }
        }



    }

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

                EditorGUI.DrawRect(selectionRect, new Color(0.5f, 0, 1));
                EditorGUI.DropShadowLabel(selectionRect, gameObject.name.Replace("-", "").ToUpperInvariant());
            }
        }
    }
#endif
}