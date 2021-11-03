using System;
using UnityEditor;
using UnityEngine;

namespace Kinogoblin.Editor.FavoriteAssets
{

    /// <summary>
    /// This class autostarts, adds items to menu, and updates Project View to match Favorites
    /// </summary>
    [InitializeOnLoad]
    public class AutoStart : MonoBehaviour
    {
        private const int ASSETMENUITEMPRIO = 18;
        private const int WINDOWMENUITEMPRIO = 12;

        private static Texture m_iconFavorite;

        static AutoStart()
        {
            //Subscribe to scene view events
            EditorApplication.update += OnEditorUpdateDelegate;
        }

        private static void OnEditorUpdateDelegate()
        {
            //Unsubscribe
            EditorApplication.update -= OnEditorUpdateDelegate;
            
            m_iconFavorite = ConfigData.Instance.FavoriteMarkerIcon;

            //Autostart window first time
            if (!EditorPrefs.HasKey(Preferences.QSPrefsAutoStart))
            {
                Preferences.AutoStart = Preferences.InitialValueAutoStart;
                Window.Init(true);
            }

            //Check editorprefs to make sure we want it to autostart
            if (Preferences.AutoStart)
            {
                Window.Init(true);
            }

            EditorApplication.projectWindowItemOnGUI += projectWindowItemOnGUI;
        }

        [MenuItem("Tools/Kinogoblin tools/FavoriteAssets/ShowWindow #T", priority = WINDOWMENUITEMPRIO)]
        private static void ShowFavoriteAssetsWindow()
        {
            Window.Init(true);
        }
        //Delegate for EditorApplication.projectWindowItemOnGUI
        private static void projectWindowItemOnGUI(string guid, Rect selectionRect)
        {
            if (ProfileData.Instance != null && ProfileData.Instance.ContainsFavoriteID(guid))
                GUI.Label(selectionRect, m_iconFavorite, GUIStyle.none);
        }

        [MenuItem("Tools/Kinogoblin tools/FavoriteAssets/Mark asset as favorite %#T", priority = WINDOWMENUITEMPRIO)]
        public static void ToggleFavorite()
        {
            Window.Init(true);
            foreach (UnityEngine.Object selectedObj in Selection.objects)
                ProfileData.Instance.ToggleAssetFavorite(selectedObj);

            foreach (UnityEngine.Object selectedObj in Selection.objects)
                ProfileData.Instance.ToggleSceneObjectFavorite(selectedObj);
        }

        [MenuItem("Assets/FavoriteAssets/Add asset to favorites", false, ASSETMENUITEMPRIO)]
        public static void AddAssetsToFavorites()
        {
            UnityEngine.Object[] objects = Selection.objects;

            foreach (UnityEngine.Object obj in objects)
                ProfileData.Instance.AddToAssetFavorites(obj);
        }

        [MenuItem("Assets/FavoriteAssets/Add asset to favorites", true, ASSETMENUITEMPRIO)]
        public static bool ValidatePrefab()
        {
            //Check if any of selected is an asset
            bool selectionContainsAssets = (Selection.assetGUIDs.Length >= 1);

            if (selectionContainsAssets)
            {
                string path;
                string id;
                foreach (UnityEngine.Object selectedObj in Selection.objects)
                {
                    //See if its already a favorite
                    path = AssetDatabase.GetAssetPath(selectedObj);
                    if (string.IsNullOrEmpty(path))
                        continue;

                    id = AssetDatabase.AssetPathToGUID(path);

                    //We found one that isn't in favorites, so we might as well just return here
                    if (!ProfileData.Instance.ContainsFavoriteID(id))
                        return true;
                }
            }
            //If they have asset IDs, they are assets
            return false;
        }

        [MenuItem("Assets/FavoriteAssets/Remove asset from favorites", false, ASSETMENUITEMPRIO)]
        public static void RemoveAssetsToFavorites()
        {
            string path;
            string id;
            foreach (UnityEngine.Object selectedObj in Selection.objects)
            {
                //See if its already a favorite
                path = AssetDatabase.GetAssetPath(selectedObj);
                id = AssetDatabase.AssetPathToGUID(path);

                //We found one in favorites, so we might as well just return here
                if (ProfileData.Instance.ContainsFavoriteID(id))
                    ProfileData.Instance.RemoveFromFavorites(id);
            }
        }

        [MenuItem("Assets/FavoriteAssets/Remove asset from favorites", true, ASSETMENUITEMPRIO)]
        public static bool ValidateCurrentFavorite()
        {
            string path;
            string id;
            foreach (UnityEngine.Object selectedObj in Selection.objects)
            {
                //See if its already a favorite
                path = AssetDatabase.GetAssetPath(selectedObj);
                id = AssetDatabase.AssetPathToGUID(path);

                //We found one in favorites, so we might as well just return here
                if (ProfileData.Instance.ContainsFavoriteID(id))
                    return true;
            }

            //None of the selected was in favorites, so return false
            return false;
        }
    }
}