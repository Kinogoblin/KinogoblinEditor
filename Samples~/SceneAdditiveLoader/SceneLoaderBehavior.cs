namespace Kinogoblin.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using UnityEngine.Playables;
    using UnityEngine.SceneManagement;

#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.SceneManagement;
#endif

    [Serializable]
    public class SceneLoaderBehavior : PlayableBehaviour
    {
        public UnityEngine.Object scenesToLoad;

        private string path;
        private bool firstFrameHappened;
        //private Material defaultMaterial;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (scenesToLoad == null)
                return;

            if (!firstFrameHappened)
            {
                firstFrameHappened = true;

#if UNITY_EDITOR

                path = AssetDatabase.GetAssetPath(scenesToLoad);
                if (!Application.isPlaying && Application.isEditor)
                {
                    EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                }
                else
                {
                    SceneManager.LoadSceneAsync(scenesToLoad.name, LoadSceneMode.Additive);
                }
#else
        
            SceneManager.LoadSceneAsync(scenesToLoad.name, LoadSceneMode.Additive);

#endif

            }

        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (firstFrameHappened)
            {
                firstFrameHappened = false;

                if (scenesToLoad == null)
                    return;


#if UNITY_EDITOR

                path = AssetDatabase.GetAssetPath(scenesToLoad);
                if (!Application.isPlaying && Application.isEditor)
                {
                    EditorSceneManager.CloseScene(SceneManager.GetSceneByPath(path), true);
                }
                else
                {
                    SceneManager.UnloadSceneAsync(scenesToLoad.name);
                }
#else
        
                SceneManager.UnloadSceneAsync(scenesToLoad.name);

#endif

                base.OnBehaviourPause(playable, info);
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (firstFrameHappened)
            {
                firstFrameHappened = false;

                if (scenesToLoad == null)
                    return;

#if UNITY_EDITOR

                path = AssetDatabase.GetAssetPath(scenesToLoad);
                if (!Application.isPlaying && Application.isEditor)
                {
                    EditorSceneManager.CloseScene(SceneManager.GetSceneByPath(path), true);
                }
                else
                {
                    SceneManager.UnloadSceneAsync(scenesToLoad.name);
                }
#else
        
                SceneManager.UnloadSceneAsync(scenesToLoad.name);

#endif

                base.OnPlayableDestroy(playable);
            }
        }

    }
}