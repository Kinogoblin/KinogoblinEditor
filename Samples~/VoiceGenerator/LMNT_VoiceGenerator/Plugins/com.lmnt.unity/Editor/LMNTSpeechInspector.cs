using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LMNT {

[CustomEditor(typeof(LMNTSpeech))]
public class LMNTSpeechInspector : Editor {
	private List<Voice> voiceList;
	private int selectedOptionIndex;
	private List<string> options;

	public void OnEnable() {
		voiceList = LMNTLoader.LoadVoices();
		options = voiceList.Select(v => v.name).ToList<string>();
	}

	public override VisualElement CreateInspectorGUI() {
		VisualElement inspector = new VisualElement();

		selectedOptionIndex = EditorGUILayout.Popup("Voice", selectedOptionIndex, options.ToArray());
		return inspector;
	}
}

}
