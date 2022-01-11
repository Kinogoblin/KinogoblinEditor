using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Sequences.Timeline;
using Kinogoblin.Playables;

namespace Kinogoblin.Playables.Timeline
{
    [CustomEditor(typeof(SceneActivationTrack))]
    class SceneActivationTrackInspector : TrackAssetInspector {}
}
