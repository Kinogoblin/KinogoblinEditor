namespace Kinogoblin.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.Video;

    public class ParticleSystemMixer : PlayableBehaviour
    {
        public ParticleSystem particleSystem;

        private bool firstFrameHappened;

        private Color defaultStartColor;
        private float defaultSimulationSpeed;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {

            particleSystem = playerData as ParticleSystem;

            if (particleSystem == null)
                return;

            if (!firstFrameHappened)
            {
                defaultStartColor = particleSystem.startColor;
                defaultSimulationSpeed = particleSystem.startDelay;
                firstFrameHappened = true;
            }

            int inputCount = playable.GetInputCount();


            Color blendedColor = Color.clear;
            float blendedSimulationSpeed = 0;

            float totalWeight = 0;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<ParticleSystemBehavior> inputPlayable = (ScriptPlayable<ParticleSystemBehavior>)playable.GetInput(i);
                ParticleSystemBehavior behavior = inputPlayable.GetBehaviour();

                blendedColor += behavior.startColor * inputWeight;
                blendedSimulationSpeed += behavior.simulationSpeed * inputWeight;

                totalWeight += inputWeight;
            }
            float remainmingWeight = 1 - totalWeight;

            particleSystem.startColor = blendedColor + defaultStartColor * remainmingWeight;
            particleSystem.startDelay = blendedSimulationSpeed + defaultSimulationSpeed * remainmingWeight;
        }

        public void Test()
        {
            
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            firstFrameHappened = false;

            if (particleSystem == null)
                return;


            particleSystem.startColor = defaultStartColor;
            particleSystem.startDelay = defaultSimulationSpeed;

        }
    }
}