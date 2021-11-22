namespace Kinogoblin.Playables
{
    using System;
    using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;

    [Serializable]
    public class LiveCameraClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField]
        private LiveCameraBehaviour template = new LiveCameraBehaviour();

        public ClipCaps clipCaps
        {
            get
            {
                return ClipCaps.Blending;
            }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<LiveCameraBehaviour>.Create(graph, template);
        }
    }
}