using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialDialogueViewButton : MonoBehaviour
{
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
		anim.Stop();
	}

	public void Init()
	{
		dialogueView.Init(tutorialDialogueSet);
	}
}
