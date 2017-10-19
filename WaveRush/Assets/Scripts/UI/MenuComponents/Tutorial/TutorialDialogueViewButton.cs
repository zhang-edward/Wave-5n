using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TutorialDialogueViewButton : MonoBehaviour
{
	public static List<string> TUTORIAL_KEYS = new List<string>();

	public Button helpButton;
	public Animator anim;
	public DialogueView dialogueView;
	public DialogueSet tutorialDialogueSet;

	private string PLAYERPREFS_KEY
	{
		get {
			return tutorialDialogueSet.name;
		}	
	}

	void Awake()
	{
		helpButton.onClick.AddListener(Init);
	}

	void OnEnable()
	{
		// If this key does not exist in PlayerPrefs, add it (should be run when the game is first initialized)
		if (!TUTORIAL_KEYS.Contains(PLAYERPREFS_KEY))
			TUTORIAL_KEYS.Add(PLAYERPREFS_KEY);
		// If the player has not seen this tutorial dialogue yet, autoplay it
		if (!PlayerPrefs.HasKey(PLAYERPREFS_KEY) || PlayerPrefs.GetInt(PLAYERPREFS_KEY) != 1)
		{
			dialogueView.onDialogueFinished += SetBounceIdleOff;
		}
		else
		{
			anim.CrossFade("nobounce", 0);
		}
	}

	private void OnDisable()
	{
		dialogueView.onDialogueFinished -= SetBounceIdleOff;
	}

	public void Init()
	{
		dialogueView.Init(tutorialDialogueSet);
	}

	private void SetBounceIdleOff()
	{
		anim.CrossFade("nobounce", 0);
		PlayerPrefs.SetInt(PLAYERPREFS_KEY, 1);
	}

	public static void ResetTutorials()
	{
		foreach (string key in TUTORIAL_KEYS)
		{
			PlayerPrefs.SetInt(key, 0);
			print("Set " + key + " to " + PlayerPrefs.GetInt(key));
		}
	}
}
