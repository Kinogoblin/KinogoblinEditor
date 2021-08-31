namespace Kinogoblin
{
    //Version 1.11
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.IO;

    enum TypesOfSettings
    {
        FolderUtils,
        SettingsForGameobject,
        ChangeMaterial,
        MultiSceneLoader,
        AddOtherPackages,
        Custom
    }

    class EditorSettings : EditorWindow
    {
        [MenuItem("Tools/Kinogoblin tools/Set Settings Window #w", false, -100)]
        public static void ShowWindow()
        {
            var window = EditorWindow.GetWindow<EditorSettings>();
        }

        private TypesOfSettings type;
        private Vector2 scrollPos = Vector2.zero;
        private string[] varaints = new string[0];
        private int selectedVariant = 0;

        void OnGUI()
        {
            titleContent = new GUIContent("K. Editor", m_Logo);
            if (varaints.Length == 0)
            {
                varaints = new string[6];
                for (int i = 0; i < 6; i++)
                {
                    varaints[i] = ((TypesOfSettings) i).ToString();
                }
            }

            if (Other.customView)
            {
                Rect graphPosition = new Rect(0f, 0f, position.width, position.height);
                GraphBackground.DrawGraphBackground(graphPosition, graphPosition);
            }

            GUILayout.Space(10f);

            GUILayout.Box("KINOGOBLIN EDITOR", Other.headerStyle, GUILayout.ExpandWidth(true), Other.headerHeight);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true));


            type = (TypesOfSettings) GUILayout.SelectionGrid((int) type, varaints, 1);

            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            switch (type)
            {
                case TypesOfSettings.ChangeMaterial:
                    ChangeMaterial.ChangeMaterialGUI();
                    break;
                case TypesOfSettings.SettingsForGameobject:
                    SettingsForGameobject.SettingsForGameobjectGUI();
                    break;
                case TypesOfSettings.FolderUtils:
                    FolderUtils.FolderUtilsGUI();
                    break;
                case TypesOfSettings.MultiSceneLoader:
                    MultiSceneLoaderGUI();
                    break;
                case TypesOfSettings.Custom:
                    Other.OtherGUI();
                    break;
                case TypesOfSettings.AddOtherPackages:
                    OtherPackages.OtherPackagesGUI();
                    break;
                default:
                    ChangeMaterial.ChangeMaterialGUI();
                    break;
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        ////////////////////
        ///Temp
        static string logoPath = "Packages/com.kinogoblin.editor/Editor/Icons/Logo.png";

        public static string settingsPath = "Packages/com.kinogoblin.editor/Editor/Data/Editor Data.asset";
        private static Texture2D m_Logo = null;

        void OnEnable()
        {
            if (AssetDatabase.LoadAssetAtPath(logoPath, typeof(Texture2D)) == null)
            {
                logoPath = "Assets/GitKinogoblin/KinogoblinEditor/Editor/Icons/Logo.png";
            }

            m_Logo = new Texture2D(16, 16, TextureFormat.PVRTC_RGBA4, false);
            m_Logo = (Texture2D) AssetDatabase.LoadAssetAtPath(logoPath, typeof(Texture2D));
            m_Logo.Apply();
        }

        public static SaveSettings GetSettings()
        {
            if (AssetDatabase.LoadAssetAtPath(settingsPath, typeof(SaveSettings)) == null)
            {
                settingsPath = "Assets/GitKinogoblin/KinogoblinEditor/Editor/Data/Editor Data.asset";
            }

            var s = (SaveSettings) AssetDatabase.LoadAssetAtPath(settingsPath, typeof(SaveSettings));

            return s;
        }

        [SerializeField] MultiSceneLoader[] custom = new MultiSceneLoader[] { };

        public void MultiSceneLoaderGUI()
        {
            GUILayout.Label("Multi Scene Loader", EditorStyles.boldLabel);

            GUILayout.Space(10f);

            ScriptableObject scriptableObj = this;
            SerializedObject serialObj = new SerializedObject(scriptableObj);
            SerializedProperty serialProp = serialObj.FindProperty("custom");

            EditorGUILayout.PropertyField(serialProp, true);
            serialObj.ApplyModifiedProperties();

            foreach (var item in custom)
            {
                if (item != null)
                {
                    GUILayout.Label("Load " + item.name, EditorStyles.boldLabel);
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Load Main Scenes"))
                        item.LoadAllScenes();

                    EditorGUILayout.Space();
                    if (GUILayout.Button("Load Set Scenes"))
                        item.LoadSetScenes(true);
                }
            }
        }
    }

    public class CustomSceneLoader : ScriptableObject
    {
        MultiSceneLoader multiSceneLoader = new MultiSceneLoader();

        [SerializeField] public UnityEngine.Object[] mainScenes = null;

        [SerializeField] public UnityEngine.Object[] setScenes = null;

        public void MultiSceneLoaderGUI()
        {
            GUILayout.Label("Multi Scene Loader", EditorStyles.boldLabel);

            GUILayout.Space(10f);

            ScriptableObject scriptableObj1 = multiSceneLoader;
            SerializedObject serialObj1 = new SerializedObject(scriptableObj1);
            SerializedProperty mainScenesProp = serialObj1.FindProperty("mainScenes");

            EditorGUILayout.PropertyField(mainScenesProp, true);
            SerializedProperty setScenesProp = serialObj1.FindProperty("setScenes");

            EditorGUILayout.PropertyField(setScenesProp, true);
            serialObj1.ApplyModifiedProperties();

            //multiSceneLoader.mainScenes = mainScenes;
            //multiSceneLoader.setScenes = setScenes;

            EditorGUILayout.Space();
            if (GUILayout.Button("Load Main Scenes", GUILayout.MinHeight(100), GUILayout.Height(50)))
                multiSceneLoader.LoadAllScenes();

            EditorGUILayout.Space();
            if (GUILayout.Button("Load Set Scenes", GUILayout.MinHeight(100), GUILayout.Height(50)))
                multiSceneLoader.LoadSetScenes(true);
        }
    }
}