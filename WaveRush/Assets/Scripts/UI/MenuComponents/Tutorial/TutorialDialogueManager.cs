using UnityEngine;
using System.Collections;

public class TutorialDialogueManager : MonoBehaviour
{
	public const string PLAYERPREFS_KEY = "Tutorial_Story_";

	public DialogueView dialogueView;
	public DialogueSet[] tutorialDialogueSets;

	public bool Init(int tutorialIndex)
	{
		if (!HasPlayedTutorial(tutorialIndex))
		{
			Debug.Log("Playing:" + tutorialIndex);
			dialogueView.Init(tutorialDialogueSets[tutorialIndex]);
			PlayerPrefs.SetInt(GetPlayerPrefsKey(tutorialIndex), 1);
			return true;
		}
		return false;
	}

	public bool HasPlayedTutorial(int tutorialIndex)
	{
		return PlayerPrefs.HasKey(GetPlayerPrefsKey(tutorialIndex)) &&
               PlayerPrefs.GetInt(GetPlayerPrefsKey(tutorialIndex)) != 0;
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
		//PlayerPrefs.SetInt(DailyHeroRewardButton.TUTORIAL_KEY, 0);
	}
}
