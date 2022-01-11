using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Sequences.Timeline;

namespace Kinogoblin.Playables
{
    /// <summary>
    /// An interface for manipulating Scenes in the context of <seealso cref="TimelineSequence"/>.
    /// </summary>
    public class SceneManagement
    {
        /// <summary>
        /// Opens the Scene located at the specified path in <seealso cref="OpenSceneMode.Additive"/>.
        /// </summary>
        /// <param name="path">Path to the Scene, relative to the project folder.</param>
        /// <param name="deactivate">Set to true to disable the root GameObjects of the loaded Scenes. Default value: false.</param>
        public static void OpenScene(string path, bool deactivate = false)
        {
            var scene = EditorSceneManager.GetSceneByPath(path);
            if (!scene.isLoaded)
                scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);

            if (deactivate)
            {
                List<GameObject> rootObjects = new List<GameObject>();
                scene.GetRootGameObjects(rootObjects);

                foreach (GameObject root in rootObjects)
                    root.SetActive(false);
            }
        }

        /// <summary>
        /// Closes the Scene located at the specified path.
        /// </summary>
        /// <param name="path">Path to the Scene, relative to the project folder.</param>
        public static void CloseScene(string path)
        {
            if (IsLoaded(path))
                EditorSceneManager.CloseScene(EditorSceneManager.GetSceneByPath(path), true);
        }

        
        /// <summary>
        /// Indicates if the Scene located at the specified path is already loaded or not in the Hierarchy.
        /// </summary>
        /// <param name="path">Path to the Scene, relative to the project folder.</param>
        /// <returns>True if the Scene located at <paramref name="path"/> is already loaded in the Hierarchy. False otherwise.</returns>
        public static bool IsLoaded(string path)
        {
            var scene = EditorSceneManager.GetSceneByPath(path);
            return scene.isLoaded;
        }
    }
}
