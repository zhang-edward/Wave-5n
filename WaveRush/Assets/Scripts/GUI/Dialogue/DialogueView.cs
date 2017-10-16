using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueView : MonoBehaviour
{
	public SimpleAnimationPlayerImage speakerImage;		// Image for the character that is speaking
	public Text nameText;								// Text containing the name of the character
	public ScrollingText dialogueText;					// Text containing the dialogue
	public bool dialoguePlaying;						// Whether or not this dialogue is currently in progress

	public delegate void DialogueLifecycleEvent();
	public event DialogueLifecycleEvent onDialogueFinished;
	
	private DialogueSet[] dialogueSets;					// Dialogue data
	private bool proceed;								// 
	private bool willAcceptScreenPress;

	public void Init(params DialogueSet[] dialogueSets)
	{
		willAcceptScreenPress = false;
		this.dialogueSets = dialogueSets;
		gameObject.SetActive(true);
		StartCoroutine(DisplayDialogues());
	}

	private IEnumerator DisplayDialogues()
	{
		dialogueText.text = "";
		dialoguePlaying = true;

		// Initialize view
		DialogueSet dialogueSet = dialogueSets[0];
		nameText.text = dialogueSet.character.characterName;
		nameText.color = dialogueSet.character.nameColor;
		dialogueText.textBox.color = dialogueSet.character.textColor;
		speakerImage.anim = dialogueSet.character.GetExpression(dialogueSet.dialogues[0].expression);
		speakerImage.Play();

		yield return new WaitForSeconds(0.5f);      // Wait for UI to animate in

		int i = 0;
		while (i < dialogueSets.Length)
		{
			dialogueSet = dialogueSets[i];
			nameText.text = dialogueSet.character.characterName;
			nameText.color = dialogueSet.character.nameColor;
			dialogueText.textBox.color = dialogueSet.character.textColor;
			dialogueText.SetScrollAudio(dialogueSet.character.voice);

			int j = 0;
			while (j < dialogueSet.dialogues.Length)
			{
				DialogueSet.Dialogue dialogue = dialogueSet.dialogues[j];
				UpdateDialogue(dialogue, dialogueSet);
				willAcceptScreenPress = false;
				while (dialogueText.IsTextScrolling())
					yield return null;
				yield return new WaitForSeconds(0.2f);		// Prevents the player from accidentally skipping things
				willAcceptScreenPress = true;
				while (!proceed)
					yield return null;
				proceed = false;
				j++;
			}
			i++;
		}
		gameObject.SetActive(false);
		dialoguePlaying = false;
		if (onDialogueFinished != null)
			onDialogueFinished();
	}

	private void UpdateDialogue(DialogueSet.Dialogue d, DialogueSet dSet)
	{
		if (d.expression != "")
		{
			speakerImage.anim = dSet.character.GetExpression(d.expression);
			speakerImage.Play();
		}
		dialogueText.UpdateText(d.text);
	}

	public void UserPressedScreen()
	{
		if (!willAcceptScreenPress)
			return;
		else
			proceed = true;
	}
}
