namespace Kinogoblin.Playables
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;
    using System;

    [CustomPropertyDrawer(typeof(ParticleSystemBehavior), true)]
    public class ParticleSystemPlayableEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // similar to before get the according serialized properties for the fieldsParticleSystemSettings
            var savedSettings = property.FindPropertyRelative("savedSettings");
            var p = property.FindPropertyRelative("teleportPosition");

            if (GUILayout.Button("CopyPasteAllComponents"))
            {
                Debug.Log("Test");
                savedSettings.boolValue = false;
            }

            // AvatarActions action = (AvatarActions)Enum.ToObject(typeof(AvatarActions), a.intValue);
            // AvatarPositions avatarPositions = (AvatarPositions)Enum.ToObject(typeof(AvatarPositions), aP.intValue);

            // Draw A field
            // EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), a);

            // draw fields indented
            EditorGUI.indentLevel++;

            position.y += EditorGUIUtility.singleLineHeight;
            // switch (action)
            // {
            //     case AvatarActions.WaveAnim:
            //         EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), l);
            //         break;
            //     case AvatarActions.IdleAnim:
            //         EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), l);
            //         break;
            //     case AvatarActions.ShowLeftAnim:
            //         EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), l);
            //         break;
            //     case AvatarActions.BlowAnim:
            //         EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), l);
            //         break;
            //     case AvatarActions.CompleteAnim:
            //         EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), l);
            //         break;
            //     case AvatarActions.Idle1:
            //         EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), l);
            //         break;
            //     case AvatarActions.Idle2:
            //         EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), l);
            //         break;
            //     case AvatarActions.Idle3:
            //         EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), l);
            //         break;
            //     case AvatarActions.Idle4:
            //         EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), l);
            //         break;
            //     case AvatarActions.Idle5:
            //         EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), l);
            //         break;
            //     case AvatarActions.Idle6:
            //         EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), l);
            //         break;
            //     default:
            //         break;
            // }

            // reset indentation
            EditorGUI.indentLevel--;
        }


        // IMPORTANT you have to implement this since your new property is
        // higher then 1 single line
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // default is 1 single line
            var height = 1;
            // if unfolded at least 1 line more, if a is true 2 lines more
            var a = property.FindPropertyRelative("action");
            // AvatarActions action = (AvatarActions)Enum.ToObject(typeof(AvatarActions), a.intValue);
            // switch (action)
            // {
            //     case AvatarActions.LoadAvatar:
            //         height += 1;
            //         break;
            //     case AvatarActions.UnloadAvatar:
            //         height += 1;
            //         break;
            //     case AvatarActions.AppeareAvatar:
            //         height += 1;
            //         break;
            //     case AvatarActions.DissapeareAvatar:
            //         height += 1;
            //         break;
            //     case AvatarActions.GiveFive:
            //         height += 1;
            //         break;
            //     case AvatarActions.ComeBack:
            //         height += 1;
            //         break;
            //     default:
            //         height += 2;
            //         break;
            // }
            return height * EditorGUIUtility.singleLineHeight;
        }
    }

    [Serializable]
    public class ParticleSystemSettings
    {
        // public AvatarActions action;
        public bool linger;
        // public AvatarPositions avatarPositions = AvatarPositions.FrontPlayer;
        public ExposedReference<Transform> teleportPosition;
    }
}