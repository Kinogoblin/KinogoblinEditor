using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using UnityEditorInternal;
using Kinogoblin.Editor;

namespace Kinogoblin.Editor.FavoriteAssets
{
    /// <summary> Kinogoblin Editor Settings

    [Serializable]
    public class HierarchyCustomColors
    {
        public string prefix = "---";
        public Color color = new Color(0.5f, 0, 1);
        public Color colorDisable = new Color(0, 0, 0, 0.5f);
        public GUIStyle style;
    }
    [Serializable]
    public class SceneHierarchy
    {
        public List<string> sceneGONames = new List<string>()
        {
             "p--Player",
             "m--Managers",
             "l--Light",
             "e--Enviroment",
             "i--Interactable",
             "s--Sound",
             "t--Timelines",
        };
    }

    [Serializable]
    public class ProjectFolderHierarchy
    {
        public string projectName = "Assets/__Project__";
        public List<string> paths = new List<string>()
        {
             "/Materials",
             "/Prefabs",
             "/Data",
             "/Prototypes",
             "/Scripts",
             "/Scenes",
             "/Trash",
             "/Animations",
             "/Animations/AnimationClips",
             "/Animations/Timelines",
             "/Editor",
             "/Audio",
             "/Models",
             "/Textures",
             "/Shaders",
        };

        public List<string> pathsSmallVersion = new List<string>()
        {
             "/Materials",
             "/Prefabs",
             "/Scripts",
             "/Scenes",
             "/Trash",
             "/Animations",
             "/Models",
             "/Textures",
        };
    }
    [Serializable]
    public class GOWithMissingScripts
    {
        public List<GameObject> gOWithMissingScripts = new List<GameObject>();
    }

    [Serializable]
    public class GameObjectCustomSettings
    {
        public GameObject renderer;
    }

    /// </summary>

    [System.Serializable]
    public class ProfileData : ScriptableObject
    {
        [HideInInspector]
        [SerializeField]
        private string m_userName;

        private History m_history;

        //[HideInInspector]
        [SerializeField]
        private Favorites m_favorites = new Favorites();

        [SerializeField]
        private SceneFavoriteManager m_sceneFavorites = new SceneFavoriteManager();

        ///////////////////////////////////
        [SerializeField]
        public List<HierarchyCustomColors> customHierarchy = new List<HierarchyCustomColors>();
        public ProjectFolderHierarchy customFolderHierarchy;
        public SceneHierarchy sceneHierarchy;
        public bool customizeHierarchy = true;

        public Color debugColor = new Color(0.5f, 0, 1);

        public bool customView = true;
        public bool customIcons = true;
		public bool dragHanglerEnable = true;
        public bool debugSend = true;
        public string pathForModels = "Assets/__Project__/Models/MeshAssets/";
        public string pathForMaterials = "Assets/__Project__/Materials/";

        public GameObjectCustomSettings gameObjectCustomSettings;
        public MeshRenderer renderer;
        public bool enableCustomImportProcessor = false;
        ///////////////////////////////////



        private static ProfileData m_instance;
        private Dictionary<string, AssetCache> m_favoriteCache;

        public static ProfileData Instance
        {
            get
            {
                if (!m_instance)
                {
                    m_instance = init();
                }
                return m_instance;
            }
        }

        internal Texture GetCachedPreview(string assetID)
        {
            if (m_favoriteCache.ContainsKey(assetID))
                return m_favoriteCache[assetID].GetCachedPreview();
            else
                return null;
        }

        internal string GetCachedObjectPath(string assetID)
        {
            if (m_favoriteCache.ContainsKey(assetID))
                return m_favoriteCache[assetID].GetCachedPath();
            else
                return null;
        }

        public int HistorySortState
        {
            get { return m_history.HistorySortState; }
            set { m_history.HistorySortState = value; }
        }

        public int SceneViewState
        {
            get { return m_sceneFavorites.ViewState; }
            set { m_sceneFavorites.ViewState = value; }
        }

        private void OnEnable()
        {
            m_history = new History();
            m_history.LoadHistory();
        }

        private static ProfileData init()
        {
            //Find all the valid resource IDs
            string[] existingProfileAssetIDs = AssetDatabase.FindAssets("t:" + typeof(ProfileData).ToString(), null);

            //Find their paths
            string[] existingProfileAssetPaths = new string[existingProfileAssetIDs.Length];
            for (int i = 0; i < existingProfileAssetIDs.Length; i++)
                existingProfileAssetPaths[i] = AssetDatabase.GUIDToAssetPath(existingProfileAssetIDs[i]);

            //Load the objects
            List<ProfileData> profileAsssets = new List<ProfileData>();
            foreach (string path in existingProfileAssetPaths)
                profileAsssets.Add(AssetDatabase.LoadAssetAtPath<ProfileData>(path));

            //Make sure we create newUser if needed
            for (int i = profileAsssets.Count - 1; i > 0; i--)
            {
                if (profileAsssets[i] == null)
                    profileAsssets.RemoveAt(i);
            }
            if (profileAsssets == null || !profileAsssets.Exists(val => val.m_userName == Environment.UserName))
                return Utils.CreateNewUserFavorite(Environment.UserName);

            //Else find the override or default user
            else
            {
                string userToFind = Preferences.GetUserOverride();
                if (!profileAsssets.Exists(val => val.m_userName == userToFind))
                    userToFind = Environment.UserName;

                //Return the proper user (Either override or Environment user)
                return profileAsssets.Find(val => val.m_userName == userToFind);
            }
        }

        internal void CleanUserData()
        {
            m_history.CleanData();
            m_favorites.CleanData();

            serializeAssetFavorites();
        }

        internal void CopyFrom(ProfileData target)
        {
            m_favorites.CopyFrom(target);
            serializeAssetFavorites();
        }

        internal List<KeyValueFavorite> GetAssetFavoritesKeyed()
        {
            return m_favorites.GetAssetFavoritesKeyed();
        }

        internal List<KeyValueFavorite> GetCurrentSceneKeyValueFavorites()
        {
            return m_sceneFavorites.GetCurrentSceneKeyValueFavorites();
        }

        internal void SwapActiveUser(ProfileData newActiveUser)
        {
            m_instance = newActiveUser;
            CleanUserData();
        }

        internal void AddToAssetFavorites(UnityEngine.Object obj)
        {
            m_favorites.AddToAssetFavorites(obj);
            serializeAssetFavorites();
        }

        private void serializeAssetFavorites()
        {
            if (m_favorites.m_IsDirty)
            {
                EditorUtility.SetDirty(this);
                ProfileData.Instance.updateCache();
                m_favorites.m_IsDirty = false;
            }
        }

        internal void ToggleAssetFavorite(UnityEngine.Object obj)
        {
            m_favorites.ToggleFavorite(obj);
            serializeAssetFavorites();
        }

        internal void ToggleSceneObjectFavorite(UnityEngine.Object selectedObj)
        {
            m_sceneFavorites.ToggleSceneObjectFavorite(selectedObj);
        }

        internal bool ContainsFavoriteID(string guid)
        {
            return m_favorites.ContainsFavoriteID(guid);
        }

        internal void RemoveFromFavorites(string guid)
        {
            m_favorites.RemoveFromFavorites(guid);
            serializeAssetFavorites();
        }

        internal void updateCache()
        {
            //Todo: Instead of redoing the dict each time, loop through it and update only what is needed!
            m_favoriteCache = new Dictionary<string, AssetCache>();
            //Loop Favorites
            foreach (var favoriteList in m_favorites.GetAssetFavoritesKeyed())
            {
                foreach (var favorite in favoriteList.Favorites)
                {
                    if (!m_favoriteCache.ContainsKey(favorite))
                    {
                        AssetCache cached = new AssetCache(favorite);
                        if (cached.IsCachable())
                            m_favoriteCache.Add(favorite, cached);
                    }
                }
            }
            //Loop history
            foreach (var historyItem in m_history.GetSmartHistory())
            {
                if (!m_favoriteCache.ContainsKey(historyItem))
                {
                    AssetCache cached = new AssetCache(historyItem);
                    if (cached.IsCachable())
                        m_favoriteCache.Add(historyItem, cached);
                }
            }
        }

        internal bool IsAllFavoriteTypesVisible()
        {
            return m_favorites.IsAllFavoriteTypesVisible();
        }

        internal bool IsAllFavoriteTypesHidden()
        {
            return m_favorites.IsAllFavoriteTypesHidden();
        }

        internal void MakeAllFavoriteTypesVisible()
        {
            m_favorites.MakeAllFavoriteTypesVisible();
        }

        internal void MakeAllFavoritesTypesHidden()
        {
            m_favorites.MakeAllFavoritesTypesHidden();
        }

        internal void SetName(string userName)
        {
            m_userName = userName;
        }

        internal string GetUserName()
        {
            return m_userName;
        }

        internal void AddToHistory(string id)
        {
            m_history.AddToHistory(id);
            updateCache();
        }

        internal void AddToHistory(UnityEngine.Object obj)
        {
            UnityEngine.Object prefabRef = Utils.GetPrefabParent(obj);

            //Check if its a instance or Asset
            string path;
            if (prefabRef != null)
                path = AssetDatabase.GetAssetPath(prefabRef);
            else
                path = AssetDatabase.GetAssetPath(obj);

            string id = AssetDatabase.AssetPathToGUID(path);

            AddToHistory(id);
        }

        internal bool HasValidSmartHistory()
        {
            return m_history.HasValidSmartHistory();
        }

        internal string[] GetSmartHistory()
        {
            return m_history.GetSmartHistory();
        }
    }
}