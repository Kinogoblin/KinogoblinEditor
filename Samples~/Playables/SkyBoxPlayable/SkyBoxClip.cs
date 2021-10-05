namespace Kinogoblin.Playables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;

    [Serializable]
    public class SkyBoxClip : PlayableAsset, ITimelineClipAsset
    {

        [SerializeField]
        private SkyBoxBehavior template = new SkyBoxBehavior();

        public ClipCaps clipCaps
        {
            get
            {
                return ClipCaps.None;
            }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<SkyBoxBehavior>.Create(graph, template);
        }
    }
}