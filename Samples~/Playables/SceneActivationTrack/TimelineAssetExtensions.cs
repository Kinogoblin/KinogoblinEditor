using System.Collections.Generic;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine;

namespace Kinogoblin.Playables.Timeline
{
    public static partial class TimelineAssetExtensions
    {
        /// <summary>
        /// Gets a collection of scene paths found in the given timeline.
        /// </summary>
        /// <param name="timeline">The instance of <see cref="TimelineAsset"/> this method applies to.</param>
        /// <returns>A read only collection of paths, relative to the project folder.</returns>
        public static IReadOnlyCollection<string> GetScenes(this TimelineAsset timeline)
        {
            List<string> paths = new List<string>();

            foreach (TrackAsset track in timeline.GetOutputTracks())
            {
                if (!(track is SceneActivationTrack) || track.muted)
                    continue;

                SceneActivationTrack scene = track as SceneActivationTrack;
                string path = scene.scene.path;

                if (string.IsNullOrEmpty(path))
                    continue;

                paths.Add(path);
            }

            return paths;
        }
    }
}
