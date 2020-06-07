namespace Kinogoblin.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using UnityEngine.Playables;

    [Serializable]
    public class SkyBoxBehavior : PlayableBehaviour
    {
        public Material newMaterial;

        private bool firstFrameHappened;
        private Material defaultMaterial;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (newMaterial == null)
                return;

            if (!firstFrameHappened)
            {
                defaultMaterial = RenderSettings.skybox;
                firstFrameHappened = true;
            }

            RenderSettings.skybox = newMaterial;
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            firstFrameHappened = false;

            if (newMaterial == null)
                return;

            //RenderSettings.skybox = defaultMaterial;
            //base.OnBehaviourPause(playable, info);
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            firstFrameHappened = false;

            if (newMaterial == null)
                return;

            //RenderSettings.skybox = defaultMaterial;
            //base.OnPlayableDestroy(playable);
        }
    }
}
