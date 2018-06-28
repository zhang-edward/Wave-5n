﻿using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Other/DialogueSet", order = 0)]
public class DialogueSet : ScriptableObject
{
	[Serializable]
	public class Dialogue
	{
		public string expression = "Default";
		[TextArea(3, 10)]
		public string text;
	}

	public DialogueCharacter character;
	[Tooltip("0 = left, 1 = right")]
	public int position;
	[Tooltip("Whether this character will exit the dialogue screen at the end of this set")]
	public bool exitAtEnd;
	[Tooltip("Whether or not this is the first time this character was introduced in the current dialogue sequence")]
	public bool firstAppearance = true;
	public Dialogue[] dialogues;
}