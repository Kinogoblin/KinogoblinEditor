using System.Linq;
using UnityEngine.Playables;

namespace Kinogoblin.Editor.Timeline
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using System.IO;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;

    public class TimelineUtilities
    {
        private static readonly GUILayoutOption buttonHeight = GUILayout.Height(30);
        private static readonly GUILayoutOption headerHeight = GUILayout.Height(25);
        private static GUIStyle buttonStyle;
        private static GUIStyle headerStyle;

        public static void TimelineUtilitiesGUI()
        {
            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle(GUI.skin.button) { richText = true };
                headerStyle = new GUIStyle(GUI.skin.box)
                    { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
            }
            
            GUILayout.Box("SETTINGS FOR TIMELINE", headerStyle, GUILayout.ExpandWidth(true), headerHeight);

            GUILayout.Space(10f);

            Transform selection = Selection.activeTransform;
            if (!IsNull(selection))
            {
                if (!IsNull(selection.GetComponentInChildren<PlayableDirector>(true)))
                {
                    if (GUILayout.Button("Copy timeline from this object: <b>" + selection.name + "</b>",
                                         buttonStyle, buttonHeight))
                        DuplicateAndSetupTimeline(selection);
                }
                else
                {
                    GUI.enabled = false;
                    GUILayout.Button("Selected object has no timeline", buttonStyle, buttonHeight);
                    GUI.enabled = true;
                }
            }
            else
            {
                GUI.enabled = false;
                GUILayout.Button("Nothing is selected", buttonStyle, buttonHeight);
                GUI.enabled = true;
            }
        }

        private static void DuplicateAndSetupTimeline(Transform selection)
        {
            var fromDirector = selection.GetComponentInChildren<PlayableDirector>(true);
            
            var fromTimeline = fromDirector.playableAsset as TimelineAsset;
            if (fromTimeline == null)
                return;

            var fromTimelinePath = AssetDatabase.GetAssetPath(fromTimeline);
            var fromTimelineFolderPath = Path.GetDirectoryName(fromTimelinePath);
            var toPrefab = new GameObject($"{selection.name}_CopyTimeline");
            toPrefab.AddComponent<PlayableDirector>();
            var toTimelinePath = Path.Combine(fromTimelineFolderPath, $"{selection.name}_CopyTimeline.playable");

            var copySucceed = AssetDatabase.CopyAsset(fromTimelinePath, toTimelinePath);
            if (!copySucceed)
                return;

            var toTimeline = AssetDatabase.LoadAssetAtPath<TimelineAsset>(toTimelinePath);

            var toDirector = toPrefab.GetComponentInChildren<PlayableDirector>(true);
            var oldBindings = fromDirector.playableAsset.outputs.ToArray();

            toDirector.playableAsset = toTimeline;
            var newBindings = toDirector.playableAsset.outputs.ToArray();

            for (int i = 0; i < oldBindings.Length; i++)
            {
                toDirector.SetGenericBinding(
                    newBindings[i].sourceObject,
                    fromDirector.GetGenericBinding(oldBindings[i].sourceObject));
            }
        }

        private static bool IsNull(Object obj)
        {
            return obj == null || obj.Equals(null);
        }
    }
}