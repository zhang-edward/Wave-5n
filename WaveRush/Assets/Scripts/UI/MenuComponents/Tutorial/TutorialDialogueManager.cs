using UnityEngine;
using System.Collections;

public class TutorialDialogueManager : MonoBehaviour
{
	public const string PLAYERPREFS_KEY = "Tutorial_MainMenu_";

	public DialogueView dialogueView;
	public DialogueSet[] dialogueSets;

	public void Init(int tutorialIndex)
	{
		if (!PlayerPrefs.HasKey(GetPlayerPrefsKey(tutorialIndex)))
		{
			dialogueView.Init(dialogueSets[0]);
		}
	}

	public string GetPlayerPrefsKey(int tutorialIndex)
	{
		return PLAYERPREFS_KEY + tutorialIndex;
	}
}
