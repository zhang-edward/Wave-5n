using UnityEngine;
using System.Collections;

public class TutorialDialogueManager : MonoBehaviour
{
	public const string PLAYERPREFS_KEY = "Tutorial_Story_";

	public DialogueView dialogueView;
	public DialogueSet[] tutorialDialogueSets;

	public void Init(int tutorialIndex)
	{
		if (!PlayerPrefs.HasKey(GetPlayerPrefsKey(tutorialIndex)) || 
		    PlayerPrefs.GetInt(GetPlayerPrefsKey(tutorialIndex)) == 0)
		{
			dialogueView.Init(tutorialDialogueSets[0]);
			PlayerPrefs.SetInt(GetPlayerPrefsKey(tutorialIndex), 1);
		}
	}

	public string GetPlayerPrefsKey(int tutorialIndex)
	{
		return PLAYERPREFS_KEY + tutorialIndex;
	}

	public void ResetTutorials()
	{
		for (int i = 0; i < tutorialDialogueSets.Length; i ++)
		{
			PlayerPrefs.SetInt(GetPlayerPrefsKey(i), 0);
		}
	}
}
