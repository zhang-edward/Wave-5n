using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(HeroPowerUpManager.HeroPowerUpDictionaryEntry))]
public class HeroPowerUpHolderDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		label = EditorGUI.BeginProperty(position, label, property);
		Rect contentPosition = EditorGUI.PrefixLabel(position, label);
		//contentPosition.width *= 0.5f;
		EditorGUI.indentLevel = 0;
		EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("powerUpPrefab"), GUIContent.none);
		contentPosition.x += contentPosition.width;
		//contentPosition.width /= 0.5f;
		EditorGUI.EndProperty();
	}
}