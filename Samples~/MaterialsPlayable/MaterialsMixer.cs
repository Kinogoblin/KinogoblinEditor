namespace Kinogoblin.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Playables;

    public class MaterialsMixer : PlayableBehaviour
    {
        public Material skybox;

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

            int inputCount = playable.GetInputCount();


            Color blendedColor = Color.clear;
            float blendedInstance = 0;
            float totalWeight = 0;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<MaterialsBehavior> inputPlayable = (ScriptPlayable<MaterialsBehavior>)playable.GetInput(i);
                MaterialsBehavior behavior = inputPlayable.GetBehaviour();

                blendedColor += behavior.color * inputWeight;
                blendedInstance += behavior.instance * inputWeight;

                totalWeight += inputWeight;
            }
            float remainmingWeight = 1 - totalWeight;

            skybox.SetColor("_Tint", blendedColor + defaultColor * remainmingWeight);
            skybox.SetFloat("_Exposure", blendedInstance + defaultInstance * remainmingWeight);

        }


        public override void OnPlayableDestroy(Playable playable)
        {
            firstFrameHappened = false;

            if (skybox == null)
                return;

            skybox.SetColor("_Tint", defaultColor);
            skybox.SetFloat("_Exposure", defaultInstance);

        }
    }
}