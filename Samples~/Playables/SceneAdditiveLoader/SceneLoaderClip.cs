namespace Kinogoblin.Playables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;

    [Serializable]
    public class SceneLoaderClip : PlayableAsset, ITimelineClipAsset
    {

        [SerializeField]
        private SceneLoaderBehavior template = new SceneLoaderBehavior();

        public ClipCaps clipCaps
        {
            get
            {
                return ClipCaps.None;
            }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<SceneLoaderBehavior>.Create(graph, template);
        }
    }
}