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
        public bool change = true;
        public bool changeOnPause = false;

        [Header("Start value")]
        public Color startColor = Color.white;
        public float startSimulationSpeed = 1;
        public float startStartLifeTime = 1;
        public Vector3 startLinear = new Vector3(0, 0, 0);
        public float startRateOverTime = 5;
        public bool looping = true;

        [Space]

        [Header("Finish value")]
        public Color finishColor = Color.black;
        public float finishSimulationSpeed = 2;
        public float finishStartLifeTime = 2;
        public Vector3 finishLinear = new Vector3(0, 0, 0);
        public float finishRateOverTime = 1920;

        private ParticleSystem particleSystem;

        private bool firstFrameHappened;

        // For save Default settings
        private Color defaultStartColor;
        private float defaultSimulationSpeed;
        private float defaultStartLifeTime;
        private Vector3 defaultLinear;
        private float defaultRateOverTime;
        private bool defaultLooping = true;

        // For save module
        private ParticleSystem.EmissionModule emissionModule;
        private ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            particleSystem = playerData as ParticleSystem;

            if (particleSystem == null)
                return;

            float totalWeight = 0;




            if (!firstFrameHappened)
            {
                // Save default value
                defaultStartColor = particleSystem.startColor;
                defaultSimulationSpeed = particleSystem.startSpeed;
                defaultStartLifeTime = particleSystem.startLifetime;

                //IF you want to change module 
                emissionModule = particleSystem.emission;
                velocityOverLifetimeModule = particleSystem.velocityOverLifetime;

                defaultRateOverTime = emissionModule.rateOverTime.constant;
                defaultLinear = new Vector3(velocityOverLifetimeModule.x.constant, velocityOverLifetimeModule.y.constant, velocityOverLifetimeModule.z.constant);

                defaultLooping = particleSystem.loop;

                firstFrameHappened = true;
            }

            totalWeight = (float)playable.GetTime() / (float)playable.GetDuration();


            float remainmingWeight = 1 - totalWeight;


            if (change)
            {
                // if change -> will change from start to finish values (bool at start will change)
                particleSystem.startColor = startColor * remainmingWeight + finishColor * totalWeight;
                particleSystem.startSpeed = startSimulationSpeed * remainmingWeight + finishSimulationSpeed * totalWeight;
                particleSystem.startLifetime = startStartLifeTime * remainmingWeight + finishStartLifeTime * totalWeight;
                //Linear 
                velocityOverLifetimeModule.x = startLinear.x * remainmingWeight + finishLinear.x * totalWeight;
                velocityOverLifetimeModule.y = startLinear.y * remainmingWeight + finishLinear.y * totalWeight;
                velocityOverLifetimeModule.z = startLinear.z * remainmingWeight + finishLinear.z * totalWeight;
                //
                emissionModule.rateOverTime = startRateOverTime * remainmingWeight + finishRateOverTime * totalWeight;
                particleSystem.loop = looping;
            }
            else
            {
                //if no change -> will give start values
                particleSystem.startColor = startColor;
                particleSystem.startSpeed = startSimulationSpeed;
                particleSystem.startLifetime = startStartLifeTime;
                //Linear 
                velocityOverLifetimeModule.x = startLinear.x;
                velocityOverLifetimeModule.y = startLinear.y;
                velocityOverLifetimeModule.z = startLinear.z;
                //
                emissionModule.rateOverTime = startRateOverTime;
                particleSystem.loop = looping;
            }

        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            firstFrameHappened = false;

            if (particleSystem == null)
                return;

            if (changeOnPause)
            {

                //give back default values on Pause and when playable end

                particleSystem.startColor = defaultStartColor;
                particleSystem.startSpeed = defaultSimulationSpeed;
                particleSystem.startLifetime = defaultStartLifeTime;
                //Linear 
                velocityOverLifetimeModule.x = defaultLinear.x;
                velocityOverLifetimeModule.y = defaultLinear.y;
                velocityOverLifetimeModule.z = defaultLinear.z;
                //
                emissionModule.rateOverTime = defaultRateOverTime;
                particleSystem.loop = defaultLooping;
            }

            base.OnBehaviourPause(playable, info);
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            firstFrameHappened = false;

            if (particleSystem == null)
                return;

            //give back default values
            particleSystem.startColor = defaultStartColor;
            particleSystem.startSpeed = defaultSimulationSpeed;
            particleSystem.startLifetime = defaultStartLifeTime;
            //Linear 
            velocityOverLifetimeModule.x = defaultLinear.x;
            velocityOverLifetimeModule.y = defaultLinear.y;
            velocityOverLifetimeModule.z = defaultLinear.z;
            //
            particleSystem.loop = defaultLooping;
            emissionModule.rateOverTime = defaultRateOverTime;

            base.OnPlayableDestroy(playable);
        }
    }
}