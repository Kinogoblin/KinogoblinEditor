namespace Kinogoblin.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;

    [TrackColor(0, 0, 0)]
    [TrackBindingType(typeof(ParticleSystem))]
    [TrackClipType(typeof(ParticleSystemClip))]
    public class ParticleSystemTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<ParticleSystemMixer>.Create(graph, inputCount);
        }
    }
}
