using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace Kinogoblin
{
    public class OtherPackages
    {

        public static Color hierarchyColor = new Color(0.5f, 0, 1);

        public static GUIStyle buttonStyle;
        public static readonly GUILayoutOption headerHeight = GUILayout.Height(25);
        public static GUIStyle headerStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter };

        public static void OtherPackagesGUI()
        {
            // ScriptableObject scriptableObj = settings;
            // SerializedObject serialObj = new SerializedObject(scriptableObj);
            // SerializedProperty serialProp = serialObj.FindProperty("customHierarchy");

            // EditorGUILayout.PropertyField(serialProp, true);
            // serialObj.ApplyModifiedProperties();

            ///////////////
            GUILayout.Box("UPDATE PACKAGE FROM UPM", headerStyle, GUILayout.ExpandWidth(true), headerHeight);

            GUILayout.Space(10f);

            if (GUILayout.Button("OpenUPMCheck"))
                OpenUPMCheck();
            if (GUILayout.Button("Add naughtyattributes"))
                AddNaughtyattributes();
            if (GUILayout.Button("Remove naughtyattributes"))
                AddNaughtyattributes();


            GUILayout.Space(10f);

        }

        static ListRequest Request;

        static void OpenUPMCheck()
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
                Debug.Log("Have OpenUPM");
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
                Request = Client.List();  // List packages installed for the Project
                EditorApplication.update += ProgressCheckKinogoblin;
            }
        }

        static void AddNaughtyattributes()
        {
            Client.Add("com.dbrizov.naughtyattributes");
            AssetDatabase.Refresh();
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