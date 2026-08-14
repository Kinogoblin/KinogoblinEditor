using System.Collections.Generic;
using Kinogoblin.Runtime;
using UnityEngine;
using UnityEditor;

namespace Kinogoblin.Editor
{
    [System.Serializable]
    public class MeshRendererSettings
    {
        public bool castShadows = true;
        public bool allowOcclusionWhenDynamic = false;
        public int renderingLayerMask = 1;
        public bool lightProbeUsage = true;
        public bool reflectionProbeUsage = true;
        public UnityEngine.Rendering.ShadowCastingMode shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

        public UnityEngine.Rendering.LightProbeUsage lightProbeUsageMode =
            UnityEngine.Rendering.LightProbeUsage.BlendProbes;

        public UnityEngine.Rendering.ReflectionProbeUsage reflectionProbeUsageMode =
            UnityEngine.Rendering.ReflectionProbeUsage.BlendProbes;

        public bool useLightProbes = true;
        public bool useReflectionProbes = true;
        public Transform anchorOverride;

    }

    public class MeshRendererEditorWindow : EditorWindow
    {
        private static MeshRendererSettings customSettings = new MeshRendererSettings();
        private static bool applyToChildren = false;

        public static void MeshRendererEditorWindowGUI()
        {
            GUILayout.Label("MeshRenderer Settings Editor", EditorStyles.boldLabel);
            GUILayout.Space(10);

            Transform[] selection = Selection.transforms;

            if (selection == null || selection.Length == 0)
            {
                EditorGUILayout.HelpBox("Please select a gameObject with MeshRenderer or parent with such objects.",
                    MessageType.Error);
            }

            GUILayout.Space(10);

            GUILayout.Label("Apply Settings", EditorStyles.boldLabel);
            applyToChildren = EditorGUILayout.Toggle("Apply to Children", applyToChildren);

            GUILayout.Space(10);

            GUILayout.Label("Set Custom MeshRenderer Settings", EditorStyles.boldLabel);

            // Shadow Casting Mode
            EditorGUILayout.BeginHorizontal();
            customSettings.shadowCastingMode = (UnityEngine.Rendering.ShadowCastingMode)EditorGUILayout.EnumPopup(
                "Shadow Casting Mode", customSettings.shadowCastingMode
            );
            if (GUILayout.Button("Apply", GUILayout.Width(60)))
            {
                ApplySettingSingleParam(selection, "shadowCastingMode");
            }

            EditorGUILayout.EndHorizontal();

            // Allow Occlusion
            EditorGUILayout.BeginHorizontal();
            customSettings.allowOcclusionWhenDynamic = EditorGUILayout.Toggle(
                "Allow Occlusion When Dynamic", customSettings.allowOcclusionWhenDynamic
            );
            if (GUILayout.Button("Apply", GUILayout.Width(60)))
            {
                ApplySettingSingleParam(selection, "allowOcclusionWhenDynamic");
            }

            EditorGUILayout.EndHorizontal();

            // Rendering Layer Mask
            EditorGUILayout.BeginHorizontal();
            customSettings.renderingLayerMask = EditorGUILayout.IntField(
                "Rendering Layer Mask", customSettings.renderingLayerMask
            );
            if (GUILayout.Button("Apply", GUILayout.Width(60)))
            {
                ApplySettingSingleParam(selection, "renderingLayerMask");
            }

            EditorGUILayout.EndHorizontal();

            // Light Probe Usage
            EditorGUILayout.BeginHorizontal();
            customSettings.lightProbeUsageMode = (UnityEngine.Rendering.LightProbeUsage)EditorGUILayout.EnumPopup(
                "Light Probe Usage", customSettings.lightProbeUsageMode
            );
            if (GUILayout.Button("Apply", GUILayout.Width(60)))
            {
                ApplySettingSingleParam(selection, "lightProbeUsage");
            }

            EditorGUILayout.EndHorizontal();

            // Reflection Probe Usage
            EditorGUILayout.BeginHorizontal();
            customSettings.reflectionProbeUsageMode = (UnityEngine.Rendering.ReflectionProbeUsage)EditorGUILayout.EnumPopup(
                "Reflection Probe Usage", customSettings.reflectionProbeUsageMode
            );
            if (GUILayout.Button("Apply", GUILayout.Width(60)))
            {
                ApplySettingSingleParam(selection, "reflectionProbeUsage");
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (GUILayout.Button("Apply All Settings"))
            {
                ApplySettingsToAll(selection);
            }
        }

        //Collects the target MeshRenderers for all selected objects, expanding into children when "Apply to Children" is on
        private static IEnumerable<MeshRenderer> GetTargetRenderers(Transform[] selection)
        {
            var result = new HashSet<MeshRenderer>();
            if (selection == null) return result;

            foreach (Transform transform in selection)
            {
                if (transform == null) continue;

                if (applyToChildren)
                {
                    foreach (var renderer in transform.GetComponentsInChildren<MeshRenderer>(true))
                        result.Add(renderer);
                }
                else
                {
                    var renderer = transform.GetComponent<MeshRenderer>();
                    if (renderer != null)
                        result.Add(renderer);
                }
            }

            return result;
        }

        private static void ApplySettingSingleParam(Transform[] selection, string paramName)
        {
            foreach (var meshRenderer in GetTargetRenderers(selection))
            {
                switch (paramName)
                {
                    case "shadowCastingMode":
                        meshRenderer.shadowCastingMode = customSettings.shadowCastingMode;
                        break;

                    case "allowOcclusionWhenDynamic":
                        meshRenderer.allowOcclusionWhenDynamic = customSettings.allowOcclusionWhenDynamic;
                        break;

                    case "renderingLayerMask":
                        meshRenderer.renderingLayerMask = (uint)customSettings.renderingLayerMask;
                        break;

                    case "lightProbeUsage":
                        meshRenderer.lightProbeUsage = customSettings.lightProbeUsageMode;
                        break;

                    case "reflectionProbeUsage":
                        meshRenderer.reflectionProbeUsage = customSettings.reflectionProbeUsageMode;
                        break;
                }

                EditorUtility.SetDirty(meshRenderer);
            }

            Helpful.Debug("Settings applied");
        }

        private static void ApplySettingsToAll(Transform[] selection)
        {
            foreach (var renderer in GetTargetRenderers(selection))
            {
                renderer.shadowCastingMode = customSettings.shadowCastingMode;
                renderer.allowOcclusionWhenDynamic = customSettings.allowOcclusionWhenDynamic;
                renderer.renderingLayerMask = (uint)customSettings.renderingLayerMask;
                renderer.lightProbeUsage = customSettings.lightProbeUsageMode;
                renderer.reflectionProbeUsage = customSettings.reflectionProbeUsageMode;
                EditorUtility.SetDirty(renderer);
            }
            Helpful.Debug("Settings applied");
        }
    }
}