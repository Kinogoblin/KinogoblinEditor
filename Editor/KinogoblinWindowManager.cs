using Kinogoblin.Editor.FavoriteAssets;
using UnityEditor;

namespace Kinogoblin.Editor
{
    public static class KinogoblinWindowManager
    {
        public static void ShowWindow()
        {
            var focusedWindow = EditorWindow.focusedWindow;
            if (ShouldOpenWindow(focusedWindow))
            {
                var window = EditorWindow.GetWindow<KinogoblinEditorWindow>("K. Editor");
                window.Show();
            }
        }

        public static void ShowWindowHotKey()
        {
            if (ProfileData.Instance.listenHotKeys)
            {
                ShowWindow();
            }
        }

        private static bool ShouldOpenWindow(EditorWindow focusedWindow)
        {
            if (!focusedWindow) return true;

            var windowTitle = focusedWindow.titleContent.text;
            return windowTitle != "Scene" && windowTitle != "Game";
        }
    }
}