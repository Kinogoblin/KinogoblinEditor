using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Kinogoblin.Editor.FavoriteAssets;
using UnityEditor.Compilation;

namespace Kinogoblin.Editor
{

	public class Other
	{
		public static Color hierarchyColor = new Color(0.5f, 0, 1);
		public static GUIStyle buttonStyle;
		public static readonly GUILayoutOption headerHeight = GUILayout.Height(25);
		public static GUIStyle headerStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter };

		public static bool customView = true;
		public static bool debugSend = true;
		private static ScriptableObject scriptableObj;
		private static SerializedObject serialObj;
		private static SerializedProperty serialProp;
		private static SerializedProperty gameObjectsWithMissingScripts;

		public static void OtherGUI()
		{
			if (scriptableObj == null)
			{
				scriptableObj = ProfileData.Instance;
				serialObj = new SerializedObject(scriptableObj);
				serialProp = serialObj.FindProperty("customHierarchy");
				gameObjectsWithMissingScripts = serialObj.FindProperty("GOWithMissingScripts");
			}
			
			///////////////
			GUILayout.Box("COLOR SETTINGS", headerStyle, GUILayout.ExpandWidth(true), headerHeight);

			GUILayout.Space(10f);

			EditorGUILayout.PropertyField(serialProp, true);
			//
			// GUILayout.Space(10f);
			//
			// EditorGUILayout.PropertyField(gameObjectsWithMissingScripts, true);

			GUILayout.Space(10f);

			ProfileData.Instance.debugColor = EditorGUILayout.ColorField("Color debug", ProfileData.Instance.debugColor);

			if (GUILayout.Button("Test Debug color"))
				Helpful.Debug("Hello from Kinogoblin!");

			GUILayout.Space(10f);

			if (GUILayout.Button("Load Kinogoblin layout"))
				LayoutLoader.LoadKinogoblinLayout();

			GUILayout.Space(10f);

			if (GUILayout.Button("Recompile Assemblies"))
				RecompileAssemblies();

			GUILayout.Space(10f);

			if (GUILayout.Button("Cleanup Missing Scripts"))
				CleanupMissingScripts();

			GUILayout.Space(10f);

			// if (GUILayout.Button("Find All Missing Scripts Objects"))
			//     CleanupMissingScripts();

			if (GUILayout.Button("Open manifest file"))
			{
				if (File.Exists(Path.GetFullPath("Packages/manifest.json")))
				{
					Application.OpenURL(Path.GetFullPath("Packages/manifest.json"));
				}
			}

			GUILayout.Space(10f);

			if (GUILayout.Button("Open packages-lock file"))
			{
				if (File.Exists(Path.GetFullPath("Packages/packages-lock.json")))
				{
					Application.OpenURL(Path.GetFullPath("Packages/packages-lock.json"));
				}
			}

			GUILayout.Space(10f);

			ProfileData.Instance.customView = EditorGUILayout.Toggle("Custom View", ProfileData.Instance.customView);
			ProfileData.Instance.customIcons = EditorGUILayout.Toggle("Custom Icons", ProfileData.Instance.customIcons);
			ProfileData.Instance.debugSend = EditorGUILayout.Toggle("Debug send", ProfileData.Instance.debugSend);
			serialObj.ApplyModifiedProperties();
		}

#if UNITY_2019_1_OR_NEWER
		[MenuItem("Tools/Kinogoblin tools/Recompile Assemblies")]
		static void RecompileAssemblies()
		{
			CompilationPipeline.RequestScriptCompilation();
		}

		[MenuItem("Tools/Kinogoblin tools/Cleanup Missing Scripts")]
		static void CleanupMissingScripts()
		{
			int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;

			for (int a = 0; a < UnityEngine.SceneManagement.SceneManager.sceneCount; a++)
			{

				var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(a);

				var rootGameObjects = scene.GetRootGameObjects();

				if (rootGameObjects != null && rootGameObjects.Length > 0)
				{

					List<GameObject> allObjectsinScene = new List<GameObject>();


					EditorUtility.DisplayProgressBar("Preprocessing", $"Fetching GameObjects in active scene \"{scene.name}\"", 0);

					foreach (var gameObject in rootGameObjects)
					{
						var childObjects = gameObject.GetComponentsInChildren<Transform>();

						if (childObjects != null && childObjects.Length > 0)
						{
							foreach (var obj in childObjects)
							{
								if (obj != null) { allObjectsinScene.Add(obj.gameObject); }
							}
						}

					}

					EditorUtility.ClearProgressBar();


					for (int b = 0; b < allObjectsinScene.Count; b++)
					{

						var gameObject = allObjectsinScene[b];

						EditorUtility.DisplayProgressBar("Removing missing script references", $"Inspecting GameObject  {b + 1}/{allObjectsinScene.Count} in active scene \"{scene.name}\"", (float)(b) / allObjectsinScene.Count);
						GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
					}

					EditorSceneManager.MarkSceneDirty(scene);

					EditorUtility.ClearProgressBar();
				}

				EditorUtility.ClearProgressBar();
			}

			EditorUtility.ClearProgressBar();

			EditorUtility.DisplayDialog("Operation Completed", "Successfully removed missing script references. Please save all currently open scenes to keep these changes persistent", "Ok");

		}
		static void FindMissingScripts()
		{
			int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;

			for (int a = 0; a < UnityEngine.SceneManagement.SceneManager.sceneCount; a++)
			{

				var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(a);

				var rootGameObjects = scene.GetRootGameObjects();

				if (rootGameObjects != null && rootGameObjects.Length > 0)
				{

					List<GameObject> allObjectsinScene = new List<GameObject>();


					EditorUtility.DisplayProgressBar("Preprocessing", $"Fetching GameObjects in active scene \"{scene.name}\"", 0);

					foreach (var gameObject in rootGameObjects)
					{
						var childObjects = gameObject.GetComponentsInChildren<Transform>();

						if (childObjects != null && childObjects.Length > 0)
						{
							foreach (var obj in childObjects)
							{
								if (obj != null) { allObjectsinScene.Add(obj.gameObject); }
							}
						}

					}

					EditorUtility.ClearProgressBar();


					for (int b = 0; b < allObjectsinScene.Count; b++)
					{

						var gameObject = allObjectsinScene[b];

						EditorUtility.DisplayProgressBar("Removing missing script references", $"Inspecting GameObject  {b + 1}/{allObjectsinScene.Count} in active scene \"{scene.name}\"", (float)(b) / allObjectsinScene.Count);
						GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
					}

					EditorSceneManager.MarkSceneDirty(scene);

					EditorUtility.ClearProgressBar();
				}

				EditorUtility.ClearProgressBar();
			}

			EditorUtility.ClearProgressBar();

			EditorUtility.DisplayDialog("Operation Completed", "Successfully removed missing script references. Please save all currently open scenes to keep these changes persistent", "Ok");

		}

#endif
		public static void UpdateScriptableObj()
		{
			scriptableObj = ProfileData.Instance;
			serialObj = new SerializedObject(scriptableObj);
			serialProp = serialObj.FindProperty("customHierarchy");
			gameObjectsWithMissingScripts = serialObj.FindProperty("GOWithMissingScripts");
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

				Helpful.Debug("Kinogoblin Editor ", $"Folder {selection.name} successfully added to .gitignore");
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
			if (ProfileData.Instance != null)
			{
				if (!ProfileData.Instance.customView) return;
				var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
				if (gameObject != null && ProfileData.Instance.customizeHierarchy)
				{
					foreach (var item in ProfileData.Instance.customHierarchy)
					{
						if (gameObject.name.StartsWith(item.prefix, System.StringComparison.Ordinal) && item.prefix != "")
						{
							var tempRect = selectionRect;
							tempRect.xMax = (tempRect.xMax * 3) / 4;
							EditorGUI.DrawRect(tempRect, item.color);
							EditorGUI.DropShadowLabel(tempRect, gameObject.name.Replace(item.prefix, "").ToUpperInvariant(), item.style);
							if (!gameObject.activeSelf)
							{
								EditorGUI.DrawRect(selectionRect, item.colorDisable);
							}
							return;
						}
					}
				}
			}

		}
	}
#endif

}
