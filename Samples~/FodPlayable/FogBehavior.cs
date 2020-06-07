namespace Kinogoblin.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using UnityEngine.Playables;

    [Serializable]
    public class FogBehavior : PlayableBehaviour
    {
        public Color color = Color.white;
        public float start = 0;
        public float end = 250;

        private bool firstFrameHappened;
        private Color defaultColor;
        private float defaultStart;
        private float defaultEnd;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {

            if (!firstFrameHappened)
            {
                defaultColor = RenderSettings.fogColor;
                defaultStart = RenderSettings.fogStartDistance;
                defaultEnd = RenderSettings.fogEndDistance;
                firstFrameHappened = true;
            }
            RenderSettings.fogColor = color;
            RenderSettings.fogStartDistance = start;
            RenderSettings.fogEndDistance = end;
        }

        //public override void OnBehaviourPause(Playable playable, FrameData info)
        //{
        //    firstFrameHappened = false;

        //    RenderSettings.fogColor = defaultColor;
        //    RenderSettings.fogStartDistance = defaultStart;
        //    RenderSettings.fogEndDistance = defaultEnd;

        //    base.OnBehaviourPause(playable, info);
        //}
    }
}