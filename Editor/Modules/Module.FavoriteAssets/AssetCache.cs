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
            var preview = AssetPreview.GetAssetPreview(obj);
            if (preview != null)
            {
                instanceID = obj.GetInstanceID();
                isCachable = true;
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