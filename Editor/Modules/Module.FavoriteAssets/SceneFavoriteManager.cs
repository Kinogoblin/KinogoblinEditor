using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Kinogoblin.Editor.FavoriteAssets
{
    [System.Serializable]
    public class SceneFavoriteManager
    {
        private int m_viewState;
        [UnityEngine.SerializeField]
        private List<SceneFavorite> m_sceneFavorites = new List<SceneFavorite>();

        SceneFavorite m_currentSceneFavorites = null;

        public int ViewState
        {
            get { return m_viewState; }
            set
            {
                //Set new sort mode and sort list accordingly
                int oldState = m_viewState;
                //Store state in prefs
                if (oldState != value)
                    EditorPrefs.SetInt(Preferences.QSPrefsHistorySortState, value);

                m_viewState = value;

                if (oldState != m_viewState)
                    updateSortedSceneFavorites();
            }
        }

        private void updateSortedSceneFavorites()
        {
            //UPDATE THE SCENE FAVORITES
            Debug.Log("UPDATE THE SCENE FAVORITES");
        }

        internal string[] getAllSceneGUIDs()
        {
            int sceneCount = EditorSceneManager.sceneCount;
            string[] sceneGUIDs = new string[sceneCount];
            for (int i = 0; i < sceneCount; i++)
            {
                sceneGUIDs[i] = getSceneGUID(EditorSceneManager.GetSceneAt(i));
            }

            return sceneGUIDs;
        }

        internal string getCurrentSceneGUID()
        {
            UnityEngine.SceneManagement.Scene scene = EditorSceneManager.GetActiveScene();
            return getSceneGUID(scene);
        }

        internal string getSceneGUID(UnityEngine.SceneManagement.Scene scene)
        {
            if (scene.IsValid())
            {
                string assetGUID = "";
                //int instanceID;
                //bool sceneFound = AssetDatabase.TryGetGUIDAndLocalFileIdentifier(AssetDatabase.LoadAssetAtPath(scene.path, typeof(UnityEngine.Object)), out assetGUID, out instanceID);
                //Return the GUID of the scene asset
                return assetGUID;
            }
            return null;
        }

        internal List<KeyValueFavorite> GetCurrentSceneKeyValueFavorites()
        {
            bool sceneHasFavorites = m_sceneFavorites.Exists(val => val.sceneAssetGUID == getCurrentSceneGUID());

            if (!sceneHasFavorites)
                return null;
            else
            {
                if (m_currentSceneFavorites == null)
                    updateCurrentSceneFavorites();

                List<KeyValueFavorite> convertedList = new List<KeyValueFavorite>();

                foreach (var item in m_currentSceneFavorites.m_SceneFavorites)
                {
                    convertedList.Add(item);
                }
                return convertedList;
            }
        }

        private void updateCurrentSceneFavorites()
        {
            //If current scene is not represented in favorites. Add it!
            SceneFavorite foo = m_sceneFavorites.Find(val => val.sceneAssetGUID == getCurrentSceneGUID());
            if (foo == null)
            {
                SceneFavorite newSceneFav = new SceneFavorite();
                newSceneFav.sceneAssetGUID = getCurrentSceneGUID();
                m_sceneFavorites.Add(newSceneFav);
            }

            m_currentSceneFavorites = m_sceneFavorites.Find(val => val.sceneAssetGUID == getCurrentSceneGUID());
        }

        internal void ToggleSceneObjectFavorite(UnityEngine.Object obj)
        {
            if (m_currentSceneFavorites == null)
                updateCurrentSceneFavorites();
            m_currentSceneFavorites.ToggleFavorite(obj);
        }
    }
}