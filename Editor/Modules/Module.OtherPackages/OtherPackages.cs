using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Kinogoblin.Runtime;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace Kinogoblin.Editor
{
    public class OtherPackages
    {
        public static Color hierarchyColor = new Color(0.5f, 0, 1);

        public static GUIStyle buttonStyle;
        public static readonly GUILayoutOption headerHeight = GUILayout.Height(25);
        public static GUIStyle headerStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter };

        public static void OtherPackagesGUI()
        {
            GUILayout.Box("UPDATE PACKAGE FROM UPM", headerStyle, GUILayout.ExpandWidth(true), headerHeight);

            GUILayout.Space(10f);

            if (GUILayout.Button("Add Open UPM"))
                AddOpenUPM();
            if (GUILayout.Button("Add kinogoblin editor from UPM"))
                AddKinogoblinEditorFromUPM();


            GUILayout.Space(10f);

            GUILayout.Box("Add other packages from UPM", headerStyle, GUILayout.ExpandWidth(true), headerHeight);

            GUILayout.Space(10f);

            if (GUILayout.Button("Add Naughtyattributes"))
                AddNaughtyattributes();
            if (GUILayout.Button("Add UnityAssetUsageDetector"))
                AddUnityAssetUsageDetector();
            if (GUILayout.Button("Add RuntimeInspector"))
                AddRuntimeInspector();
            if (GUILayout.Button("Add IngameDebugConsole"))
                AddIngameDebugConsole();
            if (GUILayout.Button("Add Graphy"))
                AddGraphy();
            if (GUILayout.Button("Add Unitask"))
                AddUnitask();
            if (GUILayout.Button("Add Unirx"))
                AddUnirx();

            GUILayout.Space(10f);
            
            GUILayout.Box("Additional functions related to plugins", headerStyle, GUILayout.ExpandWidth(true), headerHeight);

            GUILayout.Space(10f);
            
            if (GUILayout.Button("Add define: UNITASK_DOTWEEN_SUPPORT"))
                AddNewDefine(ConstData.DefineUnitaskDoTweenSupport);

        }

        static ListRequest Request;

        static void CheckUPM()
        {
            var path = Path.GetFullPath("Packages/manifest.json");

            string contents = File.ReadAllText(path);

            foreach (var item in Helpful.GetName(contents, "scopedRegistries"))
            {
                Debug.Log(item);
            }
        }

        static void AddOpenUPM()
        {
            var path = Path.GetFullPath("Packages/manifest.json");
            var path1 = Path.GetFullPath("Packages/com.kinogoblin.editor/Editor/Data");
            string contents = File.ReadAllText(path);

            if (Directory.Exists(path1))
            {
                path1 = Path.GetFullPath("Packages/com.kinogoblin.editor/Editor/Data/AddOpenUPM.txt");
            }
            else
            {
                path1 = Application.dataPath + " /GitKinogoblin/KinogoblinEditor/Editor/Data/AddOpenUPM.txt";
            }

            var helpInfo = File.ReadAllText(path1);

            if (contents.Contains("OpenUPM"))
            {
                // Debug.Log("Have OpenUPM");
            }
            else
            {
                var parts = Helpful.GetName(contents, '}');

                var newManifest = "";
                for (int i = 0; i < parts.Length - 2; i++)
                {
                    newManifest += parts[i];
                    newManifest += '}';
                }

                newManifest += helpInfo;
                File.WriteAllText(path, newManifest);
                AssetDatabase.Refresh();
            }
        }

        static void AddKinogoblinEditorFromUPM()
        {
            AddOpenUPM();
            Request = Client.List();  // List packages installed for the Project
            EditorApplication.update += ProgressCheckKinogoblin;
        }

        static void AddNaughtyattributes()
        {
            AddOpenUPM();
            AddNewPackageForUPM("com.dbrizov.naughtyattributes", "com.dbrizov.naughtyattributes");
        }

        static void AddUnityAssetUsageDetector()
        {
            AddOpenUPM();
            AddNewPackageForUPM("com.yasirkula.assetusagedetector", "com.yasirkula.assetusagedetector");
        }

        static void AddRuntimeInspector()
        {
            AddOpenUPM();
            AddNewPackageForUPM("com.yasirkula.runtimeinspector", "com.yasirkula.runtimeinspector");
        }

        static void AddIngameDebugConsole()
        {
            AddOpenUPM();
            AddNewPackageForUPM("com.yasirkula.ingamedebugconsole", "com.yasirkula.ingamedebugconsole");
        }

        static void AddGraphy()
        {
            AddOpenUPM();
            AddNewPackageForUPM("com.tayx.graphy", "com.tayx.graphy");
        }

        static void AddUnitask()
        {
            AddOpenUPM();
            AddNewPackageForUPM("com.cysharp.unitask", "com.cysharp.unitask");
        }

        static void AddUnirx()
        {
            AddOpenUPM();
            AddNewPackageForUPM("com.neuecc.unirx", "com.neuecc.unirx");
        }


        static void AddNewPackageForUPM(string scope, string name)
        {
            var path = Path.GetFullPath("Packages/manifest.json");
            var path1 = Path.GetFullPath("Packages/com.kinogoblin.editor/Editor/Data");
            string contents = File.ReadAllText(path);
            if (!contents.Contains(scope) && !contents.Contains(name))
            {
                Helpful.Debug("Try to add ", scope);
                if (Directory.Exists(path1))
                {
                    path1 = Path.GetFullPath("Packages/com.kinogoblin.editor/Editor/Data/AddOpenUPM1.txt");
                }
                else
                {
                    path1 = Application.dataPath + " /GitKinogoblin/KinogoblinEditor/Editor/Data/AddOpenUPM1.txt";
                }
                var helpInfo = File.ReadAllText(path1);

                var parts = Helpful.GetName(contents, helpInfo);
                var newManifest = parts[0];
                newManifest += "\"" + scope + "\",\n";
                newManifest += helpInfo;
                newManifest += parts[1];
                File.WriteAllText(path, newManifest);
                AssetDatabase.Refresh();
                Client.Add(name);
                AssetDatabase.Refresh();
            }
        }


        private static void AddNewDefine(string defineString)
        {
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            string[] defines = definesString.Split(' ');

            if (System.Array.IndexOf(defines, defineString) == -1)
            {
                definesString += " " + defineString;

                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, definesString);
            }
        }

        static void ProgressCheckKinogoblin()
        {
            if (Request.IsCompleted)
            {
                if (Request.Status == StatusCode.Success)
                {
                    foreach (var package in Request.Result)
                    {
                        if (package.name.Contains("com.kinogoblin.editor"))
                        {
                            Client.Remove("com.kinogoblin.editor");
                        }
                    }
                    Client.Add("com.kinogoblin.editor");
                }
                else if (Request.Status >= StatusCode.Failure)
                    Debug.Log(Request.Error.message);

                EditorApplication.update -= ProgressCheckKinogoblin;

                AssetDatabase.Refresh();
            }
        }
    }
}