namespace Kinogoblin.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;

    [Serializable]

    public class InstPrefabClip : PlayableAsset, ITimelineClipAsset
    {

        [SerializeField]
        private InstPrefabBehavior template = new InstPrefabBehavior();

        public ClipCaps clipCaps
        {
            get
            {
                return ClipCaps.None;
            }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<InstPrefabBehavior>.Create(graph, template);
        }
    }
}