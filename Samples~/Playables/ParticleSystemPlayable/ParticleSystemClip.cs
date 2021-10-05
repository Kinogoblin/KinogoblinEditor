namespace Kinogoblin.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;

    [Serializable]
    public class ParticleSystemClip : PlayableAsset, ITimelineClipAsset
    {

        [SerializeField]
        private ParticleSystemBehavior template = new ParticleSystemBehavior();

        public ClipCaps clipCaps
        {
            get
            {
                return ClipCaps.Blending;
            }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<ParticleSystemBehavior>.Create(graph, template);
        }
    }
}