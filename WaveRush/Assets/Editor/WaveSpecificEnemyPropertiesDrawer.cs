using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(StageData.WaveProperties.WaveSpecificEnemyProperties))]
public class WaveSpecificEnemyPropertiesDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		label = EditorGUI.BeginProperty(position, label, property);
	
		// Draw label and foldout menu, updated with prefab name
		SerializedProperty enemyPrefabProperty = property.FindPropertyRelative("prefab");
		string prefabName;
		if (enemyPrefabProperty.objectReferenceValue != null)
			prefabName = enemyPrefabProperty.objectReferenceValue.name;
		else
			prefabName = "Null";
		position.height = EditorGUIUtility.singleLineHeight;
		property.isExpanded = EditorGUI.Foldout (position, property.isExpanded, GUIContent.none, true);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(prefabName));

		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Foldout menu
		if (property.isExpanded) {
			// Calculate Rects
			float nextLine = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			Rect rect = new Rect(position.x, position.y + nextLine, position.width, EditorGUIUtility.singleLineHeight);
			rect.xMin *= 0.75f;
			
			// Draw fields
			EditorGUI.PropertyField(rect, enemyPrefabProperty, GUIContent.none);
			EditorGUIUtility.labelWidth = EditorGUIUtility.fieldWidth * 1.5f;
			rect.y += nextLine;
			EditorGUI.PropertyField(rect, property.FindPropertyRelative("numEnemies"), new GUIContent("Number"));
			rect.y += nextLine;
			EditorGUI.PropertyField(rect, property.FindPropertyRelative("spawnMode"), new GUIContent("Spawn Mode"));
		}
		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		// if (property.isExpanded)
		// 	return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 4;
		// else
		// 	return EditorGUIUtility.singleLineHeight;
		return EditorGUI.GetPropertyHeight(property, true);
	}
}