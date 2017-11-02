using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TutorialDialogueViewButton : MonoBehaviour
{
	public static List<string> TUTORIAL_KEYS = new List<string>();

	public Button helpButton;
	public DialogueView dialogueView;
	public DialogueSet tutorialDialogueSet;
	public NewFeatureIndicator newText;

	void Awake()
	{
		helpButton.onClick.AddListener(Init);
	}

	public void Init()
	{
		dialogueView.Init(tutorialDialogueSet);
	}
}
