using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using System;

namespace Kinogoblin
{

    [CreateAssetMenu(fileName = "Multi-Scene Loader", menuName = "KinogoblinAssets/Multi-Scene Loader", order = 2)]
    public class MultiSceneLoader : ScriptableObject
    {
        [Header("Main Scenes")]
        [SerializeField] public UnityEngine.Object[] mainScenes = null;

		[Header("Set Scenes")]
		[SerializeField] public UnityEngine.Object[] setScenes = null;

        public void LoadAllScenes()
		{
			if (mainScenes.Length == 0)
			{
				Debug.LogError("No Main scenes have been specified. Loading failed.");
				return;
			}

			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

			EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(mainScenes[0]), OpenSceneMode.Single);

			for(int i = 1; i < mainScenes.Length; i++)
            {
                if( mainScenes[i] != null)
                {
                    EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(mainScenes[i]), OpenSceneMode.Additive);
                }
                else
                {
                    Debug.Log("EpisodeConfig has empty scene!");
                }
            }

			LoadSetScenes(false);
		}

		public void LoadSetScenes(bool unloadOtherScenes)
		{
			if (setScenes.Length == 0)
			{
				if (unloadOtherScenes)
					Debug.LogError("No Set scene have been specified. Loading failed.");

				return;
			}

			OpenSceneMode mode = OpenSceneMode.Additive;

			if (unloadOtherScenes)
			{
				EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
				mode = OpenSceneMode.Single;
			}

			EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(setScenes[0]), mode);

            for (int i = 1; i < setScenes.Length; i++)
            {
                if (setScenes[i] != null)
                {
                    EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(setScenes[i]), OpenSceneMode.Additive);
                }
            }
			SetLightingSceneAsActive();
		}

		void SetLightingSceneAsActive()
		{
			for (int i = 0; i < EditorSceneManager.loadedSceneCount; i++)
			{
				Scene scene = EditorSceneManager.GetSceneAt(i);
				if (Regex.IsMatch(scene.name, "lighting", RegexOptions.IgnoreCase))
				{
					EditorSceneManager.SetActiveScene(scene);
					return;
				}
			}
		}
	}
	[Serializable]
	public class MultiSceneLoaderSer
	{
		[Header("Main Scenes")]
		[SerializeField] UnityEngine.Object[] mainScenes = null;

		[Header("Set Scenes")]
		[SerializeField] UnityEngine.Object[] setScenes = null;

		public void LoadAllScenes()
		{
			if (mainScenes.Length == 0)
			{
				Debug.LogError("No Main scenes have been specified. Loading failed.");
				return;
			}

			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

			EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(mainScenes[0]), OpenSceneMode.Single);

			for (int i = 1; i < mainScenes.Length; i++)
			{
				if (mainScenes[i] != null)
				{
					EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(mainScenes[i]), OpenSceneMode.Additive);
				}
				else
				{
					Debug.Log("EpisodeConfig has empty scene!");
				}
			}

			LoadSetScenes(false);
		}

		public void LoadSetScenes(bool unloadOtherScenes)
		{
			if (setScenes.Length == 0)
			{
				if (unloadOtherScenes)
					Debug.LogError("No Set scene have been specified. Loading failed.");

				return;
			}

			OpenSceneMode mode = OpenSceneMode.Additive;

			if (unloadOtherScenes)
			{
				EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
				mode = OpenSceneMode.Single;
			}

			EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(setScenes[0]), mode);

			for (int i = 1; i < setScenes.Length; i++)
			{
				if (setScenes[i] != null)
				{
					EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(setScenes[i]), OpenSceneMode.Additive);
				}
			}
			SetLightingSceneAsActive();
		}

		void SetLightingSceneAsActive()
		{
			for (int i = 0; i < EditorSceneManager.loadedSceneCount; i++)
			{
				Scene scene = EditorSceneManager.GetSceneAt(i);
				if (Regex.IsMatch(scene.name, "lighting", RegexOptions.IgnoreCase))
				{
					EditorSceneManager.SetActiveScene(scene);
					return;
				}
			}
		}
	}
}