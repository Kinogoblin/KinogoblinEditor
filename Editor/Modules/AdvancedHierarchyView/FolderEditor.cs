using UnityEngine;
using UnityEditor;
using Kinogoblin.Runtime;

namespace Kinogoblin.Editor
{
    [CustomEditor(typeof(Folder))]
    public class FolderEditor : UnityEditor.Editor
    {
        [MenuItem("GameObject/Kinogoblin's/Folder", false, 10)]
        static void CreateFolder()
        {
            var go = new GameObject("Folder", typeof(Folder));
            if(Selection.activeGameObject != null && Selection.activeGameObject.scene != null)
            {
                go.transform.parent = Selection.activeGameObject.transform;
            }
        }

        SerializedProperty m_Color;

        private void OnEnable()
        {
            m_Color = serializedObject.FindProperty("Color");
        }

        public override bool HasPreviewGUI()
        {
            return false;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            var color = EditorGUILayout.ColorField("Folder Color", m_Color.colorValue);
            if(EditorGUI.EndChangeCheck())
            {
                m_Color.colorValue = color;
                serializedObject.ApplyModifiedProperties();
            }
        }

    }
}

