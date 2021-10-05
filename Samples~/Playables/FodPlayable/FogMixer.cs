namespace Kinogoblin.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Playables;

    public class FogMixer : PlayableBehaviour
    {
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

            int inputCount = playable.GetInputCount();


            Color blendedColor = Color.clear;
            float blendedStart = 0;
            float blendedEnd = 0;
            float totalWeight = 0;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<FogBehavior> inputPlayable = (ScriptPlayable<FogBehavior>)playable.GetInput(i);
                FogBehavior behavior = inputPlayable.GetBehaviour();

                blendedColor += behavior.color * inputWeight;
                blendedStart += behavior.start * inputWeight;
                blendedEnd += behavior.end * inputWeight;

                totalWeight += inputWeight;
            }
            float remainmingWeight = 1 - totalWeight;

            RenderSettings.fogColor = blendedColor + defaultColor * remainmingWeight;
            RenderSettings.fogStartDistance = blendedStart + defaultStart * remainmingWeight;
            RenderSettings.fogEndDistance = blendedEnd + defaultEnd * remainmingWeight;


        }


        //public override void OnPlayableDestroy(Playable playable)
        //{
        //    firstFrameHappened = false;

        //    RenderSettings.fogColor = defaultColor;
        //    RenderSettings.fogStartDistance = defaultStart;
        //    RenderSettings.fogEndDistance = defaultEnd;

        //}
    }
}