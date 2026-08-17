using System.Collections.Generic;
using UnityEngine;

namespace Kinogoblin.Editor.FavoriteAssets
{
    //Saved Scene View camera bookmarks belonging to one scene.
    [System.Serializable]
    public class SceneCameraBookmarks
    {
        public string sceneAssetGUID;

        [SerializeField]
        private List<CameraBookmark> m_bookmarks = new List<CameraBookmark>();
        public List<CameraBookmark> Bookmarks { get { return m_bookmarks; } }

        internal void Add(CameraBookmark bookmark)
        {
            m_bookmarks.Add(bookmark);
        }

        internal void RemoveAt(int index)
        {
            if (index >= 0 && index < m_bookmarks.Count)
                m_bookmarks.RemoveAt(index);
        }

        internal void Rename(int index, string newName)
        {
            if (index >= 0 && index < m_bookmarks.Count)
                m_bookmarks[index].name = newName;
        }
    }
}
