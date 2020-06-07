using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Rendering.PostProcessing;

namespace Kinogoblin.Playables
{
    [Serializable]
    public class PostProcessBehaviour : PlayableBehaviour
    {
        public PostProcessVolume postProcessVolume;

        public override void OnPlayableDestroy(Playable playable)
        {
            if (postProcessVolume != null)
            {
                GameObject go = postProcessVolume.gameObject;
                RuntimeUtilities.DestroyVolume(postProcessVolume, false);
                RuntimeUtilities.Destroy(go);
            }
        }
    }
}