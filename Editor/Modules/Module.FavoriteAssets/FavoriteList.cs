using System.Collections.Generic;
using UnityEngine;

namespace Kinogoblin.Editor.FavoriteAssets
{
    //A user-created, named group of already-favorited asset IDs. Independent from the by-type
    //grouping in Favorites.m_favoritesKeyed - an item can belong to any number of lists.
    [System.Serializable]
    public class FavoriteList
    {
        [SerializeField]
        private string m_name;
        public string Name { get { return m_name; } set { m_name = value; } }

        [SerializeField]
        private List<string> m_itemIds = new List<string>();
        public List<string> ItemIds { get { return m_itemIds; } }

        internal bool Contains(string id)
        {
            return m_itemIds.Contains(id);
        }

        internal void Add(string id)
        {
            if (!m_itemIds.Contains(id))
                m_itemIds.Add(id);
        }

        internal void Remove(string id)
        {
            m_itemIds.Remove(id);
        }
    }
}
