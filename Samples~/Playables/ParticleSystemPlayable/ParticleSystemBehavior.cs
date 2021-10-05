namespace Kinogoblin.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine;
    using System;
    using UnityEngine.Playables;

    [Serializable]
    public class ParticleSystemBehavior : PlayableBehaviour
    {
        public Color startColor = Color.white;
        public float simulationSpeed = 1;

        public ParticleSystem particleSystemSettings = new ParticleSystem();
        public bool savedSettings = true;

        private ParticleSystem particleSystem;

        private bool firstFrameHappened;
        private Color defaultStartColor;
        private float defaultSimulationSpeed;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            particleSystem = playerData as ParticleSystem;

            if (particleSystem == null)
                return;

            if (!savedSettings)
            {
                particleSystemSettings = particleSystem;
                Debug.Log($"Saved Settings {particleSystem.startDelay} , { particleSystemSettings.startDelay} , {playerData.ToString()}");
                particleSystemSettings.startDelay = 5;
                Debug.Log($"Saved Settings {particleSystem.startDelay} , { particleSystemSettings.startDelay} , {playerData.ToString()}");
                savedSettings = true;
            }

            if (!firstFrameHappened)
            {
                defaultStartColor = particleSystem.startColor;
                defaultSimulationSpeed = particleSystem.startDelay;


                firstFrameHappened = true;
            }

            particleSystem.startColor = startColor;
            particleSystem.startDelay = simulationSpeed;

        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            firstFrameHappened = false;

            if (particleSystem == null)
                return;

            particleSystem.startColor = defaultStartColor;
            particleSystem.startDelay = defaultSimulationSpeed;

            base.OnBehaviourPause(playable, info);
        }
    }
}