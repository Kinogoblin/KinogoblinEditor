namespace Kinogoblin.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using UnityEngine.Playables;
    using UnityEngine.SceneManagement;


    [Serializable]
    public class InstPrefabBehavior : PlayableBehaviour
    {
        public GameObject prefab;
        public Vector3 position;
        public Vector3 rotation;
        
        private Transform prefabParent;
        private GameObject instPrefab;
        private bool firstFrameHappened;
        //private Material defaultMaterial;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            prefabParent = playerData as Transform;

            if (prefab == null)
                return;

            if (!firstFrameHappened)
            {
                firstFrameHappened = true;
                if (prefabParent != null)
                {
                    instPrefab = UnityEngine.Object.Instantiate(prefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), prefabParent);
                }
                else
                {
                    instPrefab = UnityEngine.Object.Instantiate(prefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                }
            }

            if (instPrefab != null)
            {
                instPrefab.transform.localPosition = position;
                instPrefab.transform.localRotation = Quaternion.Euler(rotation);
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (firstFrameHappened)
            {
                firstFrameHappened = false;

                if (prefab == null)
                    return;

                if (instPrefab != null)
                {
                    UnityEngine.Object.DestroyImmediate(instPrefab);
                }

                base.OnBehaviourPause(playable, info);
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (firstFrameHappened)
            {
                firstFrameHappened = false;

                if (prefab == null)
                    return;

                if (instPrefab != null)
                {
                    UnityEngine.Object.DestroyImmediate(instPrefab);
                }


                base.OnPlayableDestroy(playable);
            }
        }

    }
}