using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Kinogoblin.Editor.FavoriteAssets
{
    public static class Utils
    {
        public static string[] GetImmediateChildren(string path)
        {
            List<string> childrenList = new List<string>();

            // Construct the system path of the asset folder 
            string sDataPath = Application.dataPath;
            string sFolderPath = sDataPath.Substring(0, sDataPath.Length - 6) + path;
            // get the system file paths of all the files in the asset folder
            var aFilePaths = Directory.GetFiles(sFolderPath).Where(val => !val.EndsWith(".meta"));
            // enumerate through the list of files loading the assets they represent and getting their type

            foreach (string sFilePath in aFilePaths)
            {
                string sAssetPath = sFilePath.Substring(sDataPath.Length - 6).Replace("\\", "/");
                childrenList.Add(sAssetPath);
            }

            return childrenList.ToArray();
        }

        internal static ProfileData CreateNewUserFavorite(string userName)
        {
            ProfileData newUserFav = ScriptableObject.CreateInstance<ProfileData>();
            newUserFav.SetName(userName);

            string newDirName = "Users";
            var directories = Directory.GetDirectories(Application.dataPath, "Editor", SearchOption.AllDirectories);

            string parentDir = directories.FirstOrDefault<string>(val => val.Contains("Module.FavoriteAssets"));
            if (string.IsNullOrEmpty(parentDir))
                parentDir = directories.FirstOrDefault<string>(val => val.Contains("Module.FavoriteAssets"));

            string relativePath = "Assets/Resources";

            string userDir = relativePath + Path.DirectorySeparatorChar + newDirName;

            if (!AssetDatabase.IsValidFolder(userDir))
            {
                AssetDatabase.CreateFolder(relativePath, newDirName);
                return null;
            }

            string finalAssetName = userDir + "/ Profile_" + userName + ".asset";

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(finalAssetName);

            AssetDatabase.CreateAsset(newUserFav, assetPathAndName);
            AssetDatabase.SaveAssets();

            return newUserFav;
        }

        internal static GameObject FindPrefabRoot(GameObject go)
        {
#if UNITY_2018_3_OR_NEWER
            return PrefabUtility.GetOutermostPrefabInstanceRoot(go);
#else
            return PrefabUtility.FindPrefabRoot(go);
#endif
        }

        internal static UnityEngine.Object GetPrefabParent(UnityEngine.Object obj)
        {
#if UNITY_2018_3_OR_NEWER
            return PrefabUtility.GetCorrespondingObjectFromSource(obj);
#else
            return PrefabUtility.GetPrefabParent(obj);
#endif
        }
    }
}