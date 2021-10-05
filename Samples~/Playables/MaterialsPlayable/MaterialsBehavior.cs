namespace Kinogoblin.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using UnityEngine.Playables;

    [Serializable]
    public class MaterialsBehavior : PlayableBehaviour
    {
        public Color color = Color.white;
        public float instance = 1;

        private Material skybox;

        private bool firstFrameHappened;
        private Color defaultColor;
        private float defaultInstance;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            skybox = playerData as Material;

            if (skybox == null)
                return;

            if (!firstFrameHappened)
            {
                defaultColor = skybox.GetColor("_Tint");
                defaultInstance = skybox.GetFloat("_Exposure");
                firstFrameHappened = true;
            }

            skybox.SetColor("_Tint", color);
            skybox.SetFloat("_Exposure", instance);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            firstFrameHappened = false;

            if (skybox == null)
                return;

            skybox.SetColor("_Tint", defaultColor);
            skybox.SetFloat("_Exposure", defaultInstance);

            base.OnBehaviourPause(playable, info);
        }
    }
}