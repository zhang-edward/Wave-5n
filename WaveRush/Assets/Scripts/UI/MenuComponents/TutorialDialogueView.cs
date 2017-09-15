using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialDialogueView : MonoBehaviour
{
	public DialogueView dialogueView;
	public Button helpButton;

	public DialogueSet tutorialDialogueSet;

	void Awake()
	{
		helpButton.onClick.AddListener(Init);
	}

	public void Init()
	{
		dialogueView.Init(tutorialDialogueSet);
	}
}
