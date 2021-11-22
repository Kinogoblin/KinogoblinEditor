namespace Kinogoblin.Playables
{
    using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;
    using Cinemachine;

    [TrackColor(0.855f, 0.8623f, 0.870f)]
    [TrackClipType(typeof(LiveCameraClip))]
    [TrackBindingType(typeof(LiveCameraContainer))]
    public class LiveCameraTrack : TrackAsset
    {

    }
}