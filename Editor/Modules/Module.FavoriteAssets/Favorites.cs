using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using UnityEditorInternal;

namespace Kinogoblin.Editor.FavoriteAssets
{
    [System.Serializable]
    public class Favorites
    {
        public bool m_IsDirty = false;

        /// <summary>
        /// Key = AssemblyQualifiedName (Type) and Value = list of Asset IDs
        /// </summary>
        [SerializeField]
        private List<KeyValueFavorite> m_favoritesKeyed = new List<KeyValueFavorite>();

        internal void CopyFrom(ProfileData user)
        {
            foreach (KeyValueFavorite kvPair in user.GetAssetFavoritesKeyed())
            {
                foreach (string id in kvPair.Favorites)
                {
                    string path = AssetDatabase.GUIDToAssetPath(id);
                    if (!string.IsNullOrEmpty(path))
                        AddToFavorites(id, path);
                }
            }
        }

        internal void RemoveFromFavorites(string id)
        {
            for (int i = m_favoritesKeyed.Count - 1; 1 >= 0; i--)
            {
                if (m_favoritesKeyed[i].ContainsID(id))
                {
                    m_favoritesKeyed[i].RemoveFavorite(id);

                    InternalEditorUtility.RepaintAllViews();
                    m_IsDirty = true;

                    if (!m_favoritesKeyed[i].HasValidValues())
                    {
                        //Commented out since we are might be deleting element in list while its being looped
                        //m_favoritesKeyed.RemoveAt(i);
                        InternalEditorUtility.RepaintAllViews();
                        m_IsDirty = true;
                    }
                    return;
                }
            }
        }

        internal void AddToAssetFavorites(UnityEngine.Object obj)
        {

            UnityEngine.Object prefabRef = Utils.GetPrefabParent(obj);

            //CHeck if its a instance or Asset
            string path;
            if (prefabRef != null)
                path = AssetDatabase.GetAssetPath(prefabRef);
            else
                path = AssetDatabase.GetAssetPath(obj);

            string id = AssetDatabase.AssetPathToGUID(path);

            AddToFavorites(id, path);
        }

        internal void AddToFavorites(string id, string path)
        {
            //Dict
#if UNITY_2017_1_OR_NEWER
            Type type = AssetDatabase.GetMainAssetTypeAtPath(path);
#else
            Type type = AssetDatabase.LoadMainAssetAtPath(path).GetType();
#endif
            UnityEngine.Assertions.Assert.IsNotNull(type, "AH: Unable to find type for " + path);

            string typeName = type.FullName;//.AssemblyQualifiedName;
            bool isFolder = System.IO.Directory.Exists(path);
            Texture icon = EditorGUIUtility.ObjectContent(null, type).image;

            //Force a new type in dict called folder
            if (isFolder)
            {
                typeName = "Folder";
                icon = ConfigData.Instance.FolderIcon;
            }

            //New type key in dict
            if (!m_favoritesKeyed.Exists(val => val.Key == typeName))
            {
                m_favoritesKeyed.Add(new KeyValueFavorite(typeName, new List<string> { id }, icon, isFolder));
                sortList();
            }
            else
            {
                //new value in dict
                KeyValueFavorite currentValuedFavorite = m_favoritesKeyed.Find(val => val.Key == typeName);
                if (!currentValuedFavorite.ContainsID(id))
                    currentValuedFavorite.AddFavorite(id);
            }

            InternalEditorUtility.RepaintAllViews();
            m_IsDirty = true;
        }

        internal void ToggleFavorite(UnityEngine.Object obj)
        {
            UnityEngine.Object prefabRef = Utils.GetPrefabParent(obj);

            //Check if its a instance or Asset
            string path;
            if (prefabRef != null)
                path = AssetDatabase.GetAssetPath(prefabRef);
            else
                path = AssetDatabase.GetAssetPath(obj);

            string id = AssetDatabase.AssetPathToGUID(path);

            if (ContainsFavoriteID(id))
                RemoveFromFavorites(id);
            else
                AddToFavorites(id, path);
        }

        private void sortList()
        {
            m_favoritesKeyed = m_favoritesKeyed.OrderByDescending(val => val.Key).ToList();
        }

        internal List<KeyValueFavorite> GetAssetFavoritesKeyed()
        {
            return m_favoritesKeyed;
        }

        internal void CleanData()
        {
            //Favorites cleanup
            List<KeyValueFavorite> m_markedKeysForDelete = new List<KeyValueFavorite>();
            foreach (KeyValueFavorite keyedValue in m_favoritesKeyed)
            {
                List<string> m_markedItemsForDelete = new List<string>();
                foreach (string assetId in keyedValue.Favorites)
                {
                    string path = AssetDatabase.GUIDToAssetPath(assetId);

                    if (System.String.IsNullOrEmpty(path) || (!System.IO.File.Exists(path) && !System.IO.Directory.Exists(path)))
                        m_markedItemsForDelete.Add(assetId);
                }

                //Delete obsolete items
                foreach (string markedItem in m_markedItemsForDelete)
                    keyedValue.Favorites.Remove(markedItem);

                //Check key
                if (!keyedValue.HasValidValues())
                    m_markedKeysForDelete.Add(keyedValue);

                keyedValue.UpdateFilteredList();
            }
            //Delete obsolete keys
            foreach (KeyValueFavorite markedKey in m_markedKeysForDelete)
                m_favoritesKeyed.Remove(markedKey);

            ProfileData.Instance.updateCache();
        }

        internal bool ContainsFavoriteID(string guid)
        {
            foreach (KeyValueFavorite keyValue in m_favoritesKeyed)
                if (keyValue.ContainsID(guid))
                    return true;

            return false;
        }

        internal bool IsAllFavoriteTypesVisible()
        {
            return m_favoritesKeyed.All(val => val.IsHidden == false);
        }

        internal bool IsAllFavoriteTypesHidden()
        {
            return m_favoritesKeyed.All(val => val.IsHidden == true);
        }

        internal void MakeAllFavoriteTypesVisible()
        {
            m_favoritesKeyed.ForEach(val => val.IsHidden = false);
        }

        internal void MakeAllFavoritesTypesHidden()
        {
            m_favoritesKeyed.ForEach(val => val.IsHidden = true);
        }
    }
}
