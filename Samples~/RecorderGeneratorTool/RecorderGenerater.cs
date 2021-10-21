using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Recorder;
using UnityEditor.Recorder.Timeline;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
using UnityEditor.Recorder;
using UnityEditor.Presets;

namespace Kinogoblin.Tools
{
    public class RecorderGenerater : MonoBehaviour
    {
        [SerializeField] Preset preset;
        [SerializeField] public RecorderSettings defaultSettings;
        [SerializeField] private AnimationTrack _animationTrack;
        [SerializeField] private List<TimelineClip> _animationClips = new List<TimelineClip>();
        [SerializeField] private RecorderTrack _recorderTrack;
        [SerializeField] private List<RecorderClip> _recorderClips = new List<RecorderClip>();

        private TrackAsset parent;
        [Button]
        public void GetVoiceClips()
        {
            _recorderTrack = new RecorderTrack();
            var playable = GetComponent<PlayableDirector>();

            var asset = (TimelineAsset)playable.playableAsset;
            Debug.Log(asset.name);
            if (!asset) return;

            var rootTracks = asset.GetRootTracks();
            foreach (var rootTrack in rootTracks)
            {
                var _animationTrack = rootTrack is AnimationTrack;

                if (_animationTrack)
                {
                    Debug.Log(rootTrack.name);
                    foreach (var clip in rootTrack.GetClips())
                    {
                        _animationClips.Add(clip);
                    }
                    asset.CreateTrack(_recorderTrack.GetType(), null, "GeneratedRecorder");
                }
            }

            rootTracks = asset.GetRootTracks();
            foreach (var rootTrack in rootTracks)
            {
                Debug.Log(rootTrack.name);
                if (rootTrack.name == "GeneratedRecorder")
                {
                    foreach (var clip in _animationClips)
                    {
                        var recorderClip = rootTrack.CreateClip<RecorderClip>();
                        RecorderClip recorderClipAsset = recorderClip.asset as RecorderClip;
                        var clipAsset = clip.asset as AnimationPlayableAsset;
                        recorderClip.start = clip.start;
                        recorderClip.duration = clip.duration;
                        recorderClip.displayName = clipAsset.clip.name;
                        var r = (RecorderSettings)CreateFromPreset(preset);
                        r.FileNameGenerator.FileName = clipAsset.clip.name + "_<Take>";
                        recorderClipAsset.settings = r;
                    }
                }
            }

        }
        static ScriptableObject CreateFromPreset(Preset preset)
        {
            var instance = ScriptableObject.CreateInstance(preset.GetTargetFullTypeName());
            preset.ApplyTo(instance);

            return instance;
        }
    }
}