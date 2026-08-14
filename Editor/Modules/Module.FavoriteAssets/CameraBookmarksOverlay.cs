using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Kinogoblin.Editor.FavoriteAssets
{
    //Small Scene View overlay (same family as Unity's built-in "Tools" overlay) for saving and
    //jumping back to camera positions. Bookmarks are stored per-scene in ProfileData, keyed by
    //the scene asset's GUID, so they persist across sessions and follow the scene when reopened.
    [Overlay(typeof(SceneView), OverlayID, "Camera Bookmarks", true)]
    public class CameraBookmarksOverlay : IMGUIOverlay
    {
        private const string OverlayID = "Kinogoblin.CameraBookmarksOverlay";

        //Armed by the Delete toggle: the next bookmark button click deletes instead of jumping.
        //Clicking the toggle again, or clicking anywhere else in the overlay, disarms it.
        private bool m_deleteMode;

        public override void OnGUI()
        {
            SceneView sceneView = containerWindow as SceneView;
            if (sceneView == null)
                return;

            string sceneGUID = ProfileData.Instance.GetSceneGUID(EditorSceneManager.GetActiveScene());

            GUILayout.BeginVertical();

            if (string.IsNullOrEmpty(sceneGUID))
            {
                GUILayout.Label("Save the scene to store camera bookmarks.", EditorStyles.wordWrappedMiniLabel, GUILayout.Width(140));
                GUILayout.EndVertical();
                return;
            }

            SceneCameraBookmarks bookmarks = ProfileData.Instance.GetCameraBookmarks(sceneGUID);

            if (bookmarks != null)
            {
                for (int i = 0; i < bookmarks.Bookmarks.Count; i++)
                {
                    Color prevColor = GUI.backgroundColor;
                    if (m_deleteMode)
                        GUI.backgroundColor = Color.red;

                    bool clicked = GUILayout.Button("Position " + (i + 1), GUILayout.Width(140), GUILayout.Height(20));

                    GUI.backgroundColor = prevColor;

                    if (clicked)
                    {
                        if (m_deleteMode)
                        {
                            ProfileData.Instance.RemoveCameraBookmark(sceneGUID, i);
                            m_deleteMode = false;
                        }
                        else
                        {
                            goToBookmark(sceneView, bookmarks.Bookmarks[i]);
                        }
                        break;
                    }
                }
            }

            GUILayout.Space(4);

            if (GUILayout.Button("+ Add current view", GUILayout.Width(140), GUILayout.Height(20)))
                addBookmark(sceneView, sceneGUID);

            GUI.enabled = bookmarks != null && bookmarks.Bookmarks.Count > 0;
            bool wasDeleteMode = m_deleteMode;
            m_deleteMode = GUILayout.Toggle(m_deleteMode, m_deleteMode ? "Click a position to delete" : "Delete", EditorStyles.miniButton, GUILayout.Width(140), GUILayout.Height(18));
            GUI.enabled = true;

            GUILayout.EndVertical();

            //Any other click inside the overlay while armed (that a button above didn't already
            //consume) cancels delete mode instead of deleting anything.
            if (wasDeleteMode && m_deleteMode && Event.current.type == EventType.MouseDown)
            {
                m_deleteMode = false;
                Event.current.Use();
            }
        }

        private static void goToBookmark(SceneView sceneView, CameraBookmark bookmark)
        {
            sceneView.pivot = bookmark.pivot;
            sceneView.rotation = bookmark.rotation;
            sceneView.size = bookmark.size;
            sceneView.orthographic = bookmark.orthographic;
            sceneView.Repaint();
        }

        private static void addBookmark(SceneView sceneView, string sceneGUID)
        {
            CameraBookmark bookmark = new CameraBookmark
            {
                pivot = sceneView.pivot,
                rotation = sceneView.rotation,
                size = sceneView.size,
                orthographic = sceneView.orthographic
            };

            ProfileData.Instance.AddCameraBookmark(sceneGUID, bookmark);
        }
    }
}
