// using UnityEngine;
// using UnityEditor;
// using System;

// namespace Kinogoblin.Playables
// {
//     [CustomPropertyDrawer(typeof(LiveCameraBehaviour))]
//     public class LiveCameraDrawer : PropertyDrawer
//     {
//         private float fieldCount;

//         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//         {
//             SerializedProperty tweenPositionProp = property.FindPropertyRelative("cinemachineCameraActor");
//             SerializedProperty cinemachine = property.FindPropertyRelative("cinemachine");

//             position.y += 1.5f * EditorGUIUtility.singleLineHeight;

//             EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), tweenPositionProp);
//             position.y += 1.5f * EditorGUIUtility.singleLineHeight;
//             EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), cinemachine);
//             position.y += 1.5f * EditorGUIUtility.singleLineHeight;
//             fieldCount = position.y;
//         }

//         public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//         {
//             return fieldCount * (EditorGUIUtility.singleLineHeight) * 1.5f;
//         }

//     }
// }