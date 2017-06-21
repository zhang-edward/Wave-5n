using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(StageData.EnemySpawnProperties))]
public class EnemySpawnPropertiesDrawer : PropertyDrawer
{

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		label = EditorGUI.BeginProperty(position, label, property);

		// Draw label
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		// Don't make child fields be indented
		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Calculate rects
		Rect prefabRect = new Rect(position.x, position.y, position.width, 16);
		Rect propertiesRect = new Rect(position.x, position.y + 18, position.width * 0.5f, 16);

		// Draw fields
		EditorGUI.PropertyField(prefabRect, property.FindPropertyRelative("prefab"), GUIContent.none);
		EditorGUIUtility.labelWidth = 32f;
		EditorGUI.PropertyField(propertiesRect, property.FindPropertyRelative("spawnFrequency"), new GUIContent("Freq"));
		propertiesRect.x += position.width * 0.5f;
		EditorGUI.PropertyField(propertiesRect, property.FindPropertyRelative("waveLimit"), new GUIContent("Wave"));

		// Set indent back to what it was
		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return 40f;
	}
}