using UnityEditor;
using UnityEngine;

namespace Kinogoblin.Editor
{
    public static class KinogoblinResources
    {
        private const string LOGO_PACKAGE_PATH = "Packages/com.kinogoblin.editor/Editor/Icons/Logo.png";
        private const string LOGO_ASSETS_PATH = "Assets/KinogoblinEditor/Editor/Icons/Logo.png";
        
        private static Texture2D _cachedLogo;
        
        public static Texture2D GetLogo()
        {
            if (!_cachedLogo)
            {
                LoadLogo();
            }
            return _cachedLogo;
        }
        
        private static void LoadLogo()
        {
            _cachedLogo = AssetDatabase.LoadAssetAtPath<Texture2D>(LOGO_PACKAGE_PATH);
            
            if (_cachedLogo == null)
            {
                _cachedLogo = AssetDatabase.LoadAssetAtPath<Texture2D>(LOGO_ASSETS_PATH);
            }
            
            if (_cachedLogo == null)
            {
                Debug.LogWarning("[KinogoblinEditor] Logo texture not found");
            }
        }
        
        public static void ClearCache()
        {
            _cachedLogo = null;
        }
    }
}