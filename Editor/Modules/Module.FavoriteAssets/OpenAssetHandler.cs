using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Kinogoblin.Editor.FavoriteAssets
{
    public class OpenAssetHandler
    {
        [OnOpenAssetAttribute(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            ProfileData.Instance.AddToHistory(EditorUtility.InstanceIDToObject(instanceID));

            //we did not handle the open
            return false;
        }
    }
}