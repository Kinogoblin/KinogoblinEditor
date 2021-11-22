namespace Kinogoblin.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;

    [TrackColor(0, 0, 0)]
    [TrackBindingType(typeof(Material))]
    [TrackClipType(typeof(MaterialsClip))]
    public class MaterialsTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<MaterialsMixer>.Create(graph, inputCount);
        }
    }
}
