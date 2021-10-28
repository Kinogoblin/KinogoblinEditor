using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Kinogoblin.Editor.FavoriteAssets
{
    [System.Serializable]
    public class KeyValueFavorite
    {
        [SerializeField]
        [HideInInspector]
        protected string m_key;
        public string Key { get { return m_key; } set { m_key = value; } }

        [SerializeField]
        [HideInInspector]
        protected Texture m_icon;
        public Texture Icon { get { return m_icon; } set { m_icon = value; } }

        [SerializeField]
        protected List<string> m_favorites = new List<string>();
        public List<string> Favorites { get { return m_favorites; } set { m_favorites = value; } }

        protected List<string> m_filteredFavorites;

        [HideInInspector]
        [SerializeField]
        protected bool m_isFolder;
        public bool IsFolder { get { return m_isFolder; } set { m_isFolder = value; } }

        private bool m_isHidden = false;
        public bool IsHidden { get { return m_isHidden; } set { m_isHidden = value; } }

        //TODO: Seems stupid to have this in each instance of  KeyValueFavorite, abstract!!
        private string m_searchfilter = "";

        public KeyValueFavorite()
        { }

        public KeyValueFavorite(string typeName, List<string> list, Texture icon, bool isFolder)
        {
            m_key = typeName;
            m_favorites = list;
            m_icon = icon;
            m_isFolder = isFolder;
        }

        internal bool ContainsID(string id)
        {
            return m_favorites.Contains(id);
        }

        internal void AddFavorite(string id)
        {
            m_favorites.Add(id);
            sortList();
        }

        internal void RemoveFavorite(string id)
        {
            m_favorites.Remove(id);
            sortList();
        }

        virtual internal void sortList()
        {
            //Sort by the name of the asset (Fullpath if it is a folder)
            if (IsFolder)
                m_favorites = m_favorites.OrderBy(val => System.IO.Path.GetFullPath(AssetDatabase.GUIDToAssetPath(val))).ToList();
            else
                m_favorites = m_favorites.OrderBy(val => System.IO.Path.GetFileName(AssetDatabase.GUIDToAssetPath(val))).ToList();

            UpdateFilteredList();
        }

        internal bool HasValidValues()
        {
            return m_favorites != null && m_favorites.Count >= 1;
        }

        internal void SetFavoriteFilter(string searchString)
        {
            m_searchfilter = searchString.ToLowerInvariant();
            UpdateFilteredList();
        }

        virtual internal void UpdateFilteredList()
        {
            //set filteredFavorites = full list
            if (string.IsNullOrEmpty(m_searchfilter)
                || (m_key.ToLowerInvariant().Contains(m_searchfilter)))
                m_filteredFavorites = m_favorites;
            else
                //Return all that matches search
                m_filteredFavorites = m_favorites.FindAll(val => System.IO.Path.GetFileName(AssetDatabase.GUIDToAssetPath(val)).ToLowerInvariant().Contains(m_searchfilter));
        }

        internal List<string> GetFilteredFavorites()
        {
            if (m_filteredFavorites == null)
                m_filteredFavorites = m_favorites;

            return m_filteredFavorites;
        }

        internal void OnToggleSearchFilter()
        {
            m_isHidden = !m_isHidden;
        }
    }
}