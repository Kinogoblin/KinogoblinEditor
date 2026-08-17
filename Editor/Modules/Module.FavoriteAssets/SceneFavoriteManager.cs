using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kinogoblin.Editor.FavoriteAssets
{
    [System.Serializable]
    public class SceneFavoriteManager
    {
        [SerializeField]
        private List<SceneFavorite> m_sceneFavorites = new List<SceneFavorite>();

        internal static string GetSceneGUID(Scene scene)
        {
            if (!scene.IsValid() || string.IsNullOrEmpty(scene.path))
                return null;

            return AssetDatabase.AssetPathToGUID(scene.path);
        }

        internal SceneFavorite GetSceneFavorites(string sceneGUID)
        {
            if (string.IsNullOrEmpty(sceneGUID))
                return null;

            return m_sceneFavorites.Find(val => val.sceneAssetGUID == sceneGUID);
        }

        private SceneFavorite GetOrCreateSceneFavorites(string sceneGUID)
        {
            SceneFavorite entry = GetSceneFavorites(sceneGUID);
            if (entry == null)
            {
                entry = new SceneFavorite { sceneAssetGUID = sceneGUID };
                m_sceneFavorites.Add(entry);
            }
            return entry;
        }

        internal void ToggleSceneObjectFavorite(GameObject go)
        {
            string sceneGUID = GetSceneGUID(go.scene);
            if (string.IsNullOrEmpty(sceneGUID))
            {
                Debug.LogWarning("Kinogoblin Editor: Save the scene before favoriting objects in it, otherwise the reference can't be persisted.");
                return;
            }

            GetOrCreateSceneFavorites(sceneGUID).ToggleFavorite(go);
            InternalEditorUtility.RepaintAllViews();
        }

        internal void AddSceneObjectFavorite(GameObject go)
        {
            string sceneGUID = GetSceneGUID(go.scene);
            if (string.IsNullOrEmpty(sceneGUID))
            {
                Debug.LogWarning("Kinogoblin Editor: Save the scene before favoriting objects in it, otherwise the reference can't be persisted.");
                return;
            }

            GetOrCreateSceneFavorites(sceneGUID).AddFavorite(go);
            InternalEditorUtility.RepaintAllViews();
        }

        internal void RemoveSceneObjectFavorite(string sceneGUID, string globalObjectId)
        {
            GetSceneFavorites(sceneGUID)?.RemoveFavorite(globalObjectId);
            InternalEditorUtility.RepaintAllViews();
        }

        internal void CleanData()
        {
            foreach (SceneFavorite entry in m_sceneFavorites)
                entry.CleanupMissing();
        }

        //Resolved GameObject/instance-id caches go stale whenever the hierarchy structure changes
        //(objects created/destroyed/reparented, scenes loaded/unloaded).
        internal void InvalidateAllCaches()
        {
            foreach (SceneFavorite entry in m_sceneFavorites)
                entry.InvalidateCache();
        }
    }
}
