using System;
using UnityEditor;
using UnityEngine;

namespace Kinogoblin.Editor.FavoriteAssets
{
    internal class AssetCache
    {
        private bool isCachable;
        private int instanceID;
        private Texture cachedTexture;
        private string cachedPath;

        public AssetCache(string assetId)
        {
            this.cachedPath = AssetDatabase.GUIDToAssetPath(assetId);
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(this.cachedPath, typeof(UnityEngine.Object));
            if (obj != null)
            {
                instanceID = obj.GetInstanceID();
                isCachable = true;

                //Kick off preview generation now - AssetPreview.GetAssetPreview is async and often
                //returns null on this very first call while Unity renders the thumbnail in the
                //background. CheckIfReady() below keeps retrying on later repaints until it succeeds.
                cachedTexture = AssetPreview.GetAssetPreview(obj);
            }
        }

        void CheckIfReady()
        {
            if (UnityEditor.AssetPreview.IsLoadingAssetPreview(instanceID))
            {
                //Still retrieving asset preiview
            }
            else
            {
                cachedTexture = AssetPreview.GetAssetPreview(AssetDatabase.LoadAssetAtPath(this.cachedPath, typeof(UnityEngine.Object)));
            }
        }

        internal Texture GetCachedPreview()
        {
            if (cachedTexture == null) CheckIfReady();

            return cachedTexture;
        }

        internal string GetCachedPath()
        {
            return cachedPath;
        }

        internal bool IsCachable()
        {
            return isCachable;
        }
    }
}