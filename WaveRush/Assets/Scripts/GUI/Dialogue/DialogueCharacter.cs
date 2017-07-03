using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "Character", menuName = "Other/DialogueCharacter", order = 0)]
[Serializable]
public class DialogueCharacter : ScriptableObject
{
	[Serializable]
	public class CharacterExpression
	{
		public string name;
		public SimpleAnimation anim;
	}

	public string characterName;
	public Color textColor = Color.white;
	public Color nameColor = Color.white;
	public CharacterExpression[] expressions;

	public SimpleAnimation GetExpression(string name)
	{
		foreach (CharacterExpression exp in expressions)
		{
			if (exp.name.Equals(name))
				return exp.anim;
		}
		Debug.LogError("Bad shit: couldn't find expression: " + name + "!");
		return null;
	}
}