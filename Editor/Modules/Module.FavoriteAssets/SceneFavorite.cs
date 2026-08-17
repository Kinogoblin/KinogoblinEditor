using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kinogoblin.Editor.FavoriteAssets
{
    //Flat list of favorite GameObjects belonging to one scene, identified via GlobalObjectId so
    //references survive domain reloads and the scene being closed/reopened.
    [System.Serializable]
    public class SceneFavorite
    {
        public string sceneAssetGUID;

        [SerializeField]
        private List<string> m_favoriteGlobalIds = new List<string>();
        public List<string> FavoriteGlobalIds { get { return m_favoriteGlobalIds; } }

        //GlobalObjectId.GetGlobalObjectIdSlow/GlobalObjectIdentifierToObjectSlow are, as their name
        //says, slow - resolving them on every OnGUI/hierarchy row repaint is what was causing the
        //editor to lag once a scene had favorites. Resolve once, cache, and only recompute when the
        //list actually changes or the hierarchy structure changes (see AutoStart's hierarchyChanged
        //hook / ProfileData.InvalidateSceneFavoriteCaches).
        [System.NonSerialized]
        private Dictionary<string, GameObject> m_resolvedCache;
        [System.NonSerialized]
        private HashSet<int> m_resolvedInstanceIds;

        internal bool ContainsGlobalId(string globalId)
        {
            return m_favoriteGlobalIds.Contains(globalId);
        }

        internal void AddFavorite(GameObject go)
        {
            string id = GlobalObjectId.GetGlobalObjectIdSlow(go).ToString();
            if (!m_favoriteGlobalIds.Contains(id))
                m_favoriteGlobalIds.Add(id);
            InvalidateCache();
        }

        internal void RemoveFavorite(string globalId)
        {
            m_favoriteGlobalIds.Remove(globalId);
            InvalidateCache();
        }

        internal void ToggleFavorite(GameObject go)
        {
            string id = GlobalObjectId.GetGlobalObjectIdSlow(go).ToString();
            if (m_favoriteGlobalIds.Contains(id))
                m_favoriteGlobalIds.Remove(id);
            else
                m_favoriteGlobalIds.Add(id);
            InvalidateCache();
        }

        internal bool HasValidValues()
        {
            return m_favoriteGlobalIds != null && m_favoriteGlobalIds.Count > 0;
        }

        internal void InvalidateCache()
        {
            m_resolvedCache = null;
            m_resolvedInstanceIds = null;
        }

        private void EnsureCache()
        {
            if (m_resolvedCache != null)
                return;

            m_resolvedCache = new Dictionary<string, GameObject>();
            m_resolvedInstanceIds = new HashSet<int>();

            foreach (string id in m_favoriteGlobalIds)
            {
                if (!GlobalObjectId.TryParse(id, out GlobalObjectId gid))
                    continue;

                GameObject go = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(gid) as GameObject;
                if (go == null)
                    continue;

                m_resolvedCache[id] = go;
                m_resolvedInstanceIds.Add(go.GetInstanceID());
            }
        }

        internal GameObject ResolveGameObject(string globalId)
        {
            EnsureCache();
            GameObject go;
            m_resolvedCache.TryGetValue(globalId, out go);
            return go;
        }

        internal bool ContainsResolvedInstanceID(int instanceID)
        {
            EnsureCache();
            return m_resolvedInstanceIds.Contains(instanceID);
        }

        //Only purge entries when the owning scene is actually loaded and the object is confirmed
        //gone - a scene that simply isn't open right now must not lose its favorites.
        internal void CleanupMissing()
        {
            Scene scene = SceneManager.GetSceneByPath(AssetDatabase.GUIDToAssetPath(sceneAssetGUID));
            if (!scene.IsValid() || !scene.isLoaded)
                return;

            m_favoriteGlobalIds.RemoveAll(id =>
            {
                if (!GlobalObjectId.TryParse(id, out GlobalObjectId gid))
                    return true;

                return GlobalObjectId.GlobalObjectIdentifierToObjectSlow(gid) == null;
            });
            InvalidateCache();
        }
    }
}
