using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TutorialDialogueViewButton : MonoBehaviour
{
	public static List<string> TUTORIAL_KEYS = new List<string>();

	public Button helpButton;
	public DialogueView dialogueView;
	public DialogueSet[] tutorialDialogueSets;
	public NewFeatureIndicator newText;

	public string TUTORIAL_KEY {
		get { return tutorialDialogueSets[0].name; }
	}

	void Awake()
	{
		helpButton.onClick.AddListener(Init);
	}

	void Start()
	{
		newText.RegisterKey(TUTORIAL_KEY);
	}

	public void Init()
	{
		dialogueView.Init(tutorialDialogueSets);
	}
}
