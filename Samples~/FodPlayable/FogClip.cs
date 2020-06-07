namespace Kinogoblin.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;

    [Serializable]
    public class FogClip : PlayableAsset, ITimelineClipAsset
    {

        [SerializeField]
        private FogBehavior template = new FogBehavior();

        public ClipCaps clipCaps
        {
            get
            {
                return ClipCaps.Blending;
            }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<FogBehavior>.Create(graph, template);
        }
    }
}