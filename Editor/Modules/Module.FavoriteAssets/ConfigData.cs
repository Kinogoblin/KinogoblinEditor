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
            get { return getInvertedForProSkin(m_windowPaneIcon, false, ref windowPaneIconCached); }
        }

        [SerializeField]
        Texture2D m_deleteIcon = null;
        private Texture2D deleteIconCached = null;
        public Texture2D DeleteIcon
        {
            get { return getInvertedForProSkin(m_deleteIcon, false, ref deleteIconCached); }
        }

        [SerializeField]
        Texture2D m_favoriteMarkerIcon = null;
        private Texture2D favoriteMarkerIconCached = null;
        public Texture2D FavoriteMarkerIcon
        {
            get { return getInvertedForProSkin(m_favoriteMarkerIcon, false, ref favoriteMarkerIconCached); }
        }

        [SerializeField]
        Texture2D m_favoriteIcon = null;
        private Texture2D favoriteIconCached = null;
        public Texture2D FavoriteIcon
        {
            get { return getInvertedForProSkin(m_favoriteIcon, false, ref favoriteIconCached); }
        }

        [SerializeField]
        Texture2D m_folderIcon = null;
        private Texture2D folderIconCached = null;
        public Texture2D FolderIcon
        {
            get { return getInvertedForProSkin(m_folderIcon, false, ref folderIconCached); }
        }

        [SerializeField]
        Texture2D m_settingsIcon = null;
        private Texture2D settingsIconCached = null;
        public Texture2D SettingsIcon
        {
            get { return getInvertedForProSkin(m_settingsIcon, true, ref settingsIconCached); }
        }

        [SerializeField]
        Texture2D m_usersIcon = null;
        private Texture2D usersIconCached = null;
        public Texture2D UsersIcon
        {
            get { return getInvertedForProSkin(m_usersIcon, true, ref usersIconCached); }
        }

        [SerializeField]
        Texture2D m_sceneIcon = null;
        private Texture2D sceneIconCached = null;
        public Texture2D SceneIcon
        {
            get { return getInvertedForProSkin(m_sceneIcon, false, ref sceneIconCached); }
        }

        [SerializeField]
        Texture2D m_historyIcon = null;
        private Texture2D historyIconCached = null;
        public Texture2D HistoryIcon
        {
            get { return getInvertedForProSkin(m_historyIcon, true, ref historyIconCached); }
        }

        [SerializeField]
        Texture2D m_copyIcon = null;
        private Texture2D copyIconCached = null;
        public Texture2D CopyIcon
        {
            get { return getInvertedForProSkin(m_copyIcon, true, ref copyIconCached); }
        }

        [SerializeField]
        Texture2D m_plusIcon = null;
        private Texture2D plusIconCached = null;
        public Texture2D PlusIcon
        {
            get { return getInvertedForProSkin(m_plusIcon, true, ref plusIconCached); }
        }

        [SerializeField]
        Texture2D m_selectedIcon = null;
        private Texture2D selectedIconCached = null;
        public Texture2D SelectedIcon
        {
            get { return getInvertedForProSkin(m_selectedIcon, true, ref selectedIconCached); }
        }

        [SerializeField]
        Texture2D m_swapIcon = null;
        private Texture2D swapIconCached = null;
        public Texture2D SwapIcon
        {
            get { return getInvertedForProSkin(m_swapIcon, true, ref swapIconCached); }
        }

        //Inverts icon colors for pro (dark) skin and actually caches the result, so the pixel loop below
        //only ever runs once per icon per session instead of on every Window.OnEnable (window gets
        //recreated often - layout rebuilds, Maximize on Play, etc). A failure here is caught so it can't
        //silently abort whatever init code runs after this call (e.g. Window.OnEnable).
        private Texture2D getInvertedForProSkin(Texture2D orig, bool invertIfDarkSkin, ref Texture2D cache)
        {
            if (orig == null)
                return null;

            if (!invertIfDarkSkin || !EditorGUIUtility.isProSkin)
                return orig;

            if (cache != null)
                return cache;

            try
            {
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
                inverted.hideFlags = HideFlags.HideAndDontSave;
                cache = inverted;
                return inverted;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Kinogoblin Editor] Failed to invert icon '{orig.name}' for Pro Skin, falling back to original. {e.Message}");
                return orig;
            }
        }
    }
}