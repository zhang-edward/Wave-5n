using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(StageData.WaveProperties))]
public class WavePropertiesDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		label = EditorGUI.BeginProperty(position, label, property);
	
		// Draw label and foldout menu, updated with prefab name
		SerializedProperty waveNumberProperty = property.FindPropertyRelative("waveNumber");
		string waveNumber;
		if (waveNumberProperty.intValue != 0)
			waveNumber = "Wave " + waveNumberProperty.intValue;
		else
			waveNumber = "Default";
		position.height = EditorGUIUtility.singleLineHeight;
		property.isExpanded = EditorGUI.Foldout (position, property.isExpanded, GUIContent.none, true);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(waveNumber));

		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Foldout menu
		if (property.isExpanded) {
			// Calculate Rects
			float nextLine = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			Rect rect = new Rect(position.x, position.y + nextLine, position.width, EditorGUIUtility.singleLineHeight);
			rect.xMin *= 0.25f;
			
			// Draw fields
			EditorGUI.PropertyField(rect, waveNumberProperty, GUIContent.none);
			EditorGUIUtility.labelWidth = EditorGUIUtility.fieldWidth * 1.5f;
			rect.y += nextLine;
			EditorGUI.PropertyField(rect, property.FindPropertyRelative("numEnemies"), new GUIContent("Enemies"));
			rect.y += nextLine;
			EditorGUI.PropertyField(rect, property.FindPropertyRelative("maxNumActiveEnemies"), new GUIContent("Max Active"));
			rect.y += nextLine;
			EditorGUI.PropertyField(rect, property.FindPropertyRelative("enemies"), new GUIContent("SpecificEnemies") ,true);
		}
		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(property, true);
		// if (property.isExpanded)
		// 	return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 5 + property.;
		// else
		// 	return EditorGUIUtility.singleLineHeight;
	}
}