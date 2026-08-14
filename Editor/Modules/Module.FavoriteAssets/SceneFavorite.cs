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

        internal bool ContainsGlobalId(string globalId)
        {
            return m_favoriteGlobalIds.Contains(globalId);
        }

        internal void AddFavorite(GameObject go)
        {
            string id = GlobalObjectId.GetGlobalObjectIdSlow(go).ToString();
            if (!m_favoriteGlobalIds.Contains(id))
                m_favoriteGlobalIds.Add(id);
        }

        internal void RemoveFavorite(string globalId)
        {
            m_favoriteGlobalIds.Remove(globalId);
        }

        internal void ToggleFavorite(GameObject go)
        {
            string id = GlobalObjectId.GetGlobalObjectIdSlow(go).ToString();
            if (m_favoriteGlobalIds.Contains(id))
                m_favoriteGlobalIds.Remove(id);
            else
                m_favoriteGlobalIds.Add(id);
        }

        internal bool HasValidValues()
        {
            return m_favoriteGlobalIds != null && m_favoriteGlobalIds.Count > 0;
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
        }
    }
}
