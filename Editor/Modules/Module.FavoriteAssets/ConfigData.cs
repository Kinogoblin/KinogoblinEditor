using System;
using UnityEditor;
using UnityEngine;

namespace Kinogoblin.Editor.FavoriteAssets
{
    
    // [CreateAssetMenu(fileName = "ConfigData", menuName = "Kinogoblin/Data ", order = 2)]
    public class ConfigData : ScriptableObject
    {
        private static ConfigData m_instance;
        public static ConfigData Instance
        {
            get
            {
                if (!m_instance)
                {
                    m_instance = loadData();
                }

                return m_instance;
            }
        }

        private static ConfigData loadData()
        {
            //LOGO ON WINDOW
            string[] configData = AssetDatabase.FindAssets("ConfigData t: ConfigData", null);
            if (configData.Length >= 1)
            {
                return AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(configData[0]), typeof(ConfigData)) as ConfigData;
            }

            Debug.LogError("Failed to find config data");
            return null;
        }

        [SerializeField]
        Texture2D m_windowPaneIcon = null;
        private Texture2D windowPaneIconCached = null;
        public Texture2D WindowPaneIcon
        {
            get { return (windowPaneIconCached != null) ? windowPaneIconCached : getInvertedForProSkin(m_windowPaneIcon, false); }
        }

        [SerializeField]
        Texture2D m_deleteIcon = null;
        private Texture2D deleteIconCached = null;
        public Texture2D DeleteIcon
        {
            get { return (deleteIconCached != null) ? deleteIconCached : getInvertedForProSkin(m_deleteIcon, false); }
        }

        [SerializeField]
        Texture2D m_favoriteMarkerIcon = null;
        private Texture2D favoriteMarkerIconCached = null;
        public Texture2D FavoriteMarkerIcon
        {
            get { return (favoriteMarkerIconCached != null) ? favoriteMarkerIconCached : getInvertedForProSkin(m_favoriteMarkerIcon, false); }
        }

        [SerializeField]
        Texture2D m_favoriteIcon = null;
        private Texture2D favoriteIconCached = null;
        public Texture2D FavoriteIcon
        {
            get { return (favoriteIconCached != null) ? favoriteIconCached : getInvertedForProSkin(m_favoriteIcon, false); }
        }

        [SerializeField]
        Texture2D m_folderIcon = null;
        private Texture2D folderIconCached = null;
        public Texture2D FolderIcon
        {
            get { return (folderIconCached != null) ? folderIconCached : getInvertedForProSkin(m_folderIcon, false); }
        }

        [SerializeField]
        Texture2D m_settingsIcon = null;
        private Texture2D settingsIconCached = null;
        public Texture2D SettingsIcon
        {
            get { return (settingsIconCached != null) ? settingsIconCached : getInvertedForProSkin(m_settingsIcon, true); }
        }

        [SerializeField]
        Texture2D m_usersIcon = null;
        private Texture2D usersIconCached = null;
        public Texture2D UsersIcon
        {
            get { return (usersIconCached != null) ? usersIconCached : getInvertedForProSkin(m_usersIcon, true); }
        }

        [SerializeField]
        Texture2D m_sceneIcon = null;
        private Texture2D sceneIconCached = null;
        public Texture2D SceneIcon
        {
            get { return (sceneIconCached != null) ? sceneIconCached : getInvertedForProSkin(m_sceneIcon, false); }
        }

        [SerializeField]
        Texture2D m_historyIcon = null;
        private Texture2D historyIconCached = null;
        public Texture2D HistoryIcon
        {
            get { return (historyIconCached != null) ? historyIconCached : getInvertedForProSkin(m_historyIcon, true); }
        }

        [SerializeField]
        Texture2D m_copyIcon = null;
        private Texture2D copyIconCached = null;
        public Texture2D CopyIcon
        {
            get { return (copyIconCached != null) ? copyIconCached : getInvertedForProSkin(m_copyIcon, true); }
        }

        [SerializeField]
        Texture2D m_plusIcon = null;
        private Texture2D plusIconCached = null;
        public Texture2D PlusIcon
        {
            get { return (plusIconCached != null) ? plusIconCached : getInvertedForProSkin(m_plusIcon, true); }
        }

        [SerializeField]
        Texture2D m_selectedIcon = null;
        private Texture2D selectedIconCached = null;
        public Texture2D SelectedIcon
        {
            get { return (selectedIconCached != null) ? selectedIconCached : getInvertedForProSkin(m_selectedIcon, true); }
        }

        [SerializeField]
        Texture2D m_swapIcon = null;
        private Texture2D swapIconCached = null;
        public Texture2D SwapIcon
        {
            get { return (swapIconCached != null) ? swapIconCached : getInvertedForProSkin(m_swapIcon, true); }
        }

        private Texture2D getInvertedForProSkin(Texture2D orig, bool invertIfDarkSkin)
        {
            if (!invertIfDarkSkin || !EditorGUIUtility.isProSkin)
                return orig;

            Texture2D inverted = new Texture2D(orig.width, orig.height, TextureFormat.ARGB32, false);
            for (int x = 0; x < orig.width; x++)
            {
                for (int y = 0; y < orig.height; y++)
                {
                    Color origColor = orig.GetPixel(x, y);
                    Color invertedColor = new Color(1 - origColor.r, 1 - origColor.g, 1 - origColor.b, origColor.a);
                    inverted.SetPixel(x, y, (origColor.a > 0) ? invertedColor : origColor);
                }
            }
            inverted.Apply();
            return inverted;
        }
    }
}