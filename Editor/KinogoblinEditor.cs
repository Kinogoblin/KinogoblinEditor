namespace Kinogoblin
{
    //Version 1.10
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
        Other
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

        void OnGUI()
        {
            titleContent = new GUIContent("K. Editor", m_Logo);

            Rect graphPosition = new Rect(0f, 0f, position.width, position.height);
            GraphBackground.DrawGraphBackground(graphPosition, graphPosition);

            GUILayout.Space(10f);

            GUILayout.Box("KINOGOBLIN EDITOR", Other.headerStyle, GUILayout.ExpandWidth(true), Other.headerHeight);

            type = (TypesOfSettings)EditorGUILayout.EnumPopup("Primitive to create:", type);


            GUILayout.Space(10f);

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
                case TypesOfSettings.Other:
                    Other.OtherGUI();
                    break;
                default:
                    ChangeMaterial.ChangeMaterialGUI();
                    break;
            }
            EditorGUILayout.EndScrollView();
        }

        ////////////////////
        ///Temp
        static string logoPath = EditorUtilities.packagePathRoot + "/Editor/Icons/Logo.png";

        private static Texture2D m_Logo = null;
        void OnEnable()
        {
            if (!File.Exists(logoPath))
            {
                logoPath = Application.dataPath + "/GitKinogoblin/KinogoblinEditor/Editor/Icons/Logo.png";
            }
            if (File.Exists(logoPath))
            {
                Helpful.Debug(logoPath);
            }
            m_Logo = new Texture2D(16, 16, TextureFormat.PVRTC_RGBA4, false);
            var b = File.ReadAllBytes(logoPath);
            m_Logo.LoadImage(b);
            m_Logo.Apply();
        }
        
        /// <summary>
        /// To do change location for multi scene
        /// </summary>

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