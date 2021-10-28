using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kinogoblin.Editor.FavoriteAssets
{
    public class ProjectData
    {
        public List<string> m_sceneAssetsIDs;

        internal void Init()
        {

            analyzeScene(SceneManager.GetActiveScene());
            //analyzeProject(); This probably should be an action from the user to traverse the project and get usable data since it might be slow
        }

        private void analyzeScene(Scene scene)
        {
            //TODO this might be a bit excessive to run each time the heirarchy change. Look into it. Maybe on a timer, or maybe add a refresh button (Could also be done with History actually)
            List<GameObject> rootObjects = new List<GameObject>();
            scene.GetRootGameObjects(rootObjects);

            Dictionary<string, int> sceneAssetGUIDDict = new Dictionary<string, int>();

            // iterate root objects and do something
            for (int i = 0; i < rootObjects.Count; ++i)
            {
                addToProjectDataRecursively(ref sceneAssetGUIDDict, rootObjects[i]);
            }

            //Sort Dictionary
            var sortedDict = (from entry in sceneAssetGUIDDict orderby entry.Value descending select entry);

            //Create list from sorted dict
            m_sceneAssetsIDs = new List<string>();
            foreach (var kvPair in sortedDict)
            {
                m_sceneAssetsIDs.Add(kvPair.Key);
            }
        }

        private void addToProjectDataRecursively(ref Dictionary<string, int> sceneAssetDict, GameObject curGameObject)
        {
            //Check if this is a prefab
            UnityEngine.Object prefabRef = Utils.GetPrefabParent(curGameObject);
            string identifier = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(prefabRef));

            if (prefabRef != null)
            {
                //Update dict
                if (!sceneAssetDict.ContainsKey(identifier))
                    sceneAssetDict.Add(identifier, 1);

                //Update usageCount
                sceneAssetDict[identifier]++;
            }

            //Loop children
            foreach (Transform child in curGameObject.transform)
            {
                //If this child is part of the same prefab then dont recurse through it. Problem is models with submeshes being counted several times
                GameObject childGameobjectParent = Utils.FindPrefabRoot(child.gameObject);
                GameObject currentGameobjectParent = Utils.FindPrefabRoot(curGameObject.gameObject);

                if (childGameobjectParent != currentGameobjectParent)
                    addToProjectDataRecursively(ref sceneAssetDict, child.gameObject);
            }
        }

        internal void UpdateSceneInfo()
        {
            analyzeScene(SceneManager.GetActiveScene());
        }

        internal List<string> GetSceneAssetIdentifiers()
        {
            return m_sceneAssetsIDs;
        }

        internal bool HasValidSceneData()
        {
            return m_sceneAssetsIDs != null;
        }

        internal void VerifyData()
        {
            List<string> m_markedItemsForDelete = new List<string>();
            foreach (string assetId in m_sceneAssetsIDs)
            {
                if (String.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(assetId)))
                    m_markedItemsForDelete.Add(assetId);
            }

            //Delete obsolete items
            foreach (string markedItem in m_markedItemsForDelete)
                m_sceneAssetsIDs.Remove(markedItem);

        }
    }
}