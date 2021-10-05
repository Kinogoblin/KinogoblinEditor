namespace Kinogoblin.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;

    [Serializable]
    public class MaterialsClip : PlayableAsset, ITimelineClipAsset
    {

        [SerializeField]
        private MaterialsBehavior template = new MaterialsBehavior();

        public ClipCaps clipCaps
        {
            get
            {
                return ClipCaps.Blending;
            }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<MaterialsBehavior>.Create(graph, template);
        }
    }
}