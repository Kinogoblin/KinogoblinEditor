using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Kinogoblin.Editor.FavoriteAssets
{
    [System.Serializable]
    public class SceneCameraBookmarkManager
    {
        [SerializeField]
        private List<SceneCameraBookmarks> m_sceneBookmarks = new List<SceneCameraBookmarks>();

        internal SceneCameraBookmarks GetBookmarks(string sceneGUID)
        {
            if (string.IsNullOrEmpty(sceneGUID))
                return null;

            return m_sceneBookmarks.Find(val => val.sceneAssetGUID == sceneGUID);
        }

        internal SceneCameraBookmarks GetOrCreateBookmarks(string sceneGUID)
        {
            SceneCameraBookmarks entry = GetBookmarks(sceneGUID);
            if (entry == null)
            {
                entry = new SceneCameraBookmarks { sceneAssetGUID = sceneGUID };
                m_sceneBookmarks.Add(entry);
            }
            return entry;
        }

        internal void RemoveBookmark(string sceneGUID, int index)
        {
            GetBookmarks(sceneGUID)?.RemoveAt(index);
            InternalEditorUtility.RepaintAllViews();
        }

        internal void RenameBookmark(string sceneGUID, int index, string newName)
        {
            GetBookmarks(sceneGUID)?.Rename(index, newName);
            InternalEditorUtility.RepaintAllViews();
        }

        internal void CleanData()
        {
            //Drop bookmark groups for scenes that no longer exist on disk
            m_sceneBookmarks.RemoveAll(val => string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(val.sceneAssetGUID)));
        }
    }
}
