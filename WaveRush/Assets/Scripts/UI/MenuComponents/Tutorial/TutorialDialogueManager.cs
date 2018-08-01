using UnityEngine;
using System.Collections;

public class TutorialDialogueManager : MonoBehaviour 
{
	[System.Serializable]
	public struct TutorialDialogueSetInfo {
		public int series;
		public int stage;
		public DialogueSet[] dialogueSets;
	}

	public const string PLAYERPREFS_KEY = "Tutorial_Story_";

	public DialogueView dialogueView;
	public TutorialDialogueSetInfo[] tutorialDialogueSets;

	private GameManager gm;

	private void OnEnable() {
		gm = GameManager.instance;
		gm.OnDeletedData += ResetTutorials;
	}

	private void OnDisable() {
		gm.OnDeletedData -= ResetTutorials;
	}

	public bool Init() {
		for (int i = 0; i < tutorialDialogueSets.Length; i ++) {
			TutorialDialogueSetInfo info = tutorialDialogueSets[i];
			if (CheckStage(info.series, info.stage)) {
				print ("Trying to play " + i);
				if (PlayDialogue(i))
					return true;
			}
		}
		return false;
	}

	public bool PlayDialogue(int tutorialIndex) {
		if (!HasPlayedTutorial(tutorialIndex)) {
			print ("Playing " + tutorialIndex);
			dialogueView.Init(tutorialDialogueSets[tutorialIndex].dialogueSets);
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

	public string GetPlayerPrefsKey(int tutorialIndex) {
		return PLAYERPREFS_KEY + tutorialIndex;
	}

	public void ResetTutorials()
	{
		print("Tutorials reset");
		for (int i = 0; i < tutorialDialogueSets.Length; i ++) {
			PlayerPrefs.SetInt(GetPlayerPrefsKey(i), 0);
		}
	}

	private bool CheckStage(int series, int stage) {
		return (gm.save.LatestSeriesIndex == series &&
		        gm.save.LatestStageIndex == stage);
	}
}
