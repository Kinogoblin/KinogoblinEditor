using UnityEngine;
using UnityEditor;

namespace Kinogoblin.Editor.FavoriteAssets
{
    internal class UIStyler
    {
        internal static void DrawAssetFromID(string assetID, bool showPreview)
        {
            UnityEngine.Object cachedObject = AssetDatabase.LoadAssetAtPath(ProfileData.Instance.GetCachedObjectPath(assetID), typeof(UnityEngine.Object));

            //This shows AssetPreview
            GUILayout.BeginHorizontal();

            if (showPreview && cachedObject != null)
            {
                Texture cached = ProfileData.Instance.GetCachedPreview(assetID);
                if (cached != null)
                {
                    //Calculate a proper height to make sure it fits within the bounds, and has not additional spacing
                    float ratio = ((float)(cached.height) / (float)(cached.width));

                    //Make sure that tall previews doesn't break the layout
                    if (ratio > 1) ratio = 1;
                    GUILayout.Label(cached, GUILayout.MaxHeight(Mathf.Max(Preferences.ThumbnailMinSize, Preferences.ThumbnailSize * ratio)), GUILayout.Width(Preferences.ThumbnailSize));
                }
            }

            GUI.SetNextControlName(assetID);
            //If the object path has not been cached just find it using its guid
            EditorGUILayout.ObjectField((cachedObject != null) ? cachedObject : AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(assetID)), typeof(UnityEngine.Object), false);

            //EditorGUIUtility.labelWidth = 0;

            //Adding right click functionality
            AddGenericMenu("Remove from favorites", assetID);
            GUILayout.EndHorizontal();

            //Do drag functionality
            Event e = Event.current;
            bool hasMouseFocus = GUILayoutUtility.GetLastRect().Contains(e.mousePosition);
            if (hasMouseFocus && e.type == EventType.MouseDrag)
            {
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.objectReferences = new UnityEngine.Object[] { cachedObject };
                DragAndDrop.StartDrag("drag");
                e.Use();
            }
            if (e.type == EventType.Repaint)
            {
                SceneView.RepaintAll();
            }
        }

        public static void DeleteFavoriteCallback(object id)
        {
            ProfileData.Instance.RemoveFromFavorites(id as string);
        }

        internal static void AddGenericMenu(string text, string id)
        {
            Event e = Event.current;
            if (e.type == EventType.ContextClick)
            {
                bool hasMouseFocus = GUILayoutUtility.GetLastRect().Contains(e.mousePosition);
                if (hasMouseFocus)
                {
                    // Now create the menu, add items and show it
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent(text), false, UIStyler.DeleteFavoriteCallback, id);
                    menu.ShowAsContext();
                    e.Use();
                }
            }
        }
    }
}