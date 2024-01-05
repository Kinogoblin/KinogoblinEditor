using System.Collections.Generic;
using System.Linq;
using Kinogoblin.Samples;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LMNT {

[CustomEditor(typeof(LMNTSpeechCustom))]
public class LMNTSpeechCustomInspector : Editor {
	private List<Voice> voiceList;
	private int selectedOptionIndex = 0;
	private List<string> options = new();
	private SerializedProperty selectedOptionProperty;


	public void OnEnable() {
		voiceList = LMNTLoader.LoadVoices();
		options = voiceList.Select(v => v.name).ToList<string>();
		selectedOptionProperty = serializedObject.FindProperty("voice");
		selectedOptionIndex = options.FindIndex(voice => voice == selectedOptionProperty.stringValue);
	}
	public override void OnInspectorGUI()
	{
		// base.OnInspectorGUI();
		serializedObject.Update();
		selectedOptionIndex = EditorGUILayout.Popup("Voice", selectedOptionIndex, options.ToArray());
		if (selectedOptionIndex == -1)
			selectedOptionIndex = 0;
		selectedOptionProperty.stringValue = options[selectedOptionIndex];
		serializedObject.ApplyModifiedProperties();
	}
}

}
