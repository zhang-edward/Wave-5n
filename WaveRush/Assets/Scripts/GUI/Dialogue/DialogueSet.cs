﻿﻿using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Other/DialogueSet", order = 0)]
public class DialogueSet : ScriptableObject
{
	[Serializable]
	public class Dialogue
	{
		public string expression;
		[TextArea(3, 10)]
		public string text;
	}

	public DialogueCharacter character;
	public Dialogue[] dialogues;
}