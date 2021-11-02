using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Recorder;
using UnityEditor.Recorder.Timeline;
//using Sirenix.OdinInspector;
using UnityEngine.Playables;
using UnityEditor.Recorder;
using UnityEditor.Presets;
using Kinogoblin.Editor;

namespace UnityEditor.Recorder.Timeline
{
    public class RecorderGenerater : MonoBehaviour
    {
        [SerializeField] Preset preset;
        [SerializeField] string prefix;
        [SerializeField]  double gap = 5f;
        
        private List<TimelineClip> _animationClips = new List<TimelineClip>();
        private RecorderTrack _recorderTrack;
        private List<RecorderSettings> recorderSettings = new List<RecorderSettings>();
        private List<RecorderSettings> recorderSettingsAssets = new List<RecorderSettings>();

        private TrackAsset parent;

        //[Button]
        [ContextMenu("GenerateAssets")]
        public void GeneratedRecorderAssets()
        {
            _recorderTrack = new RecorderTrack();
            var playable = GetComponent<PlayableDirector>();
            _animationClips = new List<TimelineClip>();
            foreach (var item in recorderSettings)
            {
                DestroyImmediate(item);
            }
            foreach (var item in recorderSettingsAssets)
            {
                DestroyImmediate(item);
            }
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
                    var trackClips = rootTrack.GetClips();
                    
                    foreach (var clip in rootTrack.GetClips())
                    {
                        
                        _animationClips.Add(clip);
                        

                    }

                    for (int i = 0; i < _animationClips.Count-1; i++)
                    {
                        TimelineClip currClip = _animationClips[i];
                        TimelineClip nextClip = _animationClips[i+1];
                        double temp = nextClip.start - currClip.end;
                        
                        if (temp <= 0)
                        {
                            nextClip.start =currClip.end + gap;
                        }
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
                        var rec = CreateAsset(r,recorderClipAsset);
                        var name = clipAsset.clip.name;
                        name = "[" + Helpful.GetName(name,'[',2);
                        recorderClip.displayName = prefix + name;
                        recorderSettings.Add(r);
                        recorderSettingsAssets.Add(rec);
                        rec.FileNameGenerator.FileName = prefix + name + "_<Take>";
                        rec.name = prefix + name + "_<Take>";
                        recorderClipAsset.settings = rec;
                    }
                }
            }

        }

        private RecorderSettings CreateAsset(RecorderSettings recorderSettings,RecorderClip clip)
        {
            var recorder = (RecorderSettings)ScriptableObject.CreateInstance(recorderSettings.GetType());
            recorder = recorderSettings;
            AssetDatabase.AddObjectToAsset(recorder, clip);
            Undo.RegisterCreatedObjectUndo(recorder, "Recorded Settings Created");
            AssetDatabase.SaveAssets();
            return recorder;
        }

        static ScriptableObject CreateFromPreset(Preset preset)
        {
            var instance = ScriptableObject.CreateInstance(preset.GetTargetFullTypeName());
            preset.ApplyTo(instance);

            return instance;
        }
    }
}