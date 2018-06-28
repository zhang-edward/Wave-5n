using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class DialogueView : MonoBehaviour
{
	private readonly string[] SLIDE_IN_STATE = new string[] { "SlideInLeft", "SlideInRight" };
	private readonly string[] NEW_EXPRESSION_STATE = new string[] { "NewExpressionLeft", "NewExpressionRight" };

	public Image[] speakerImages;			// Image for the character that is speaking,  0 = left, 1 = right
	public TMP_Text[] nameTexts;			// Text containing the name of the character, 0 = left, 1 = right
	public CanvasGroup[] namePanels;
	public ScrollingText dialogueText;		// Text containing the dialogue
	public bool dialoguePlaying;			// Whether or not this dialogue is currently in progress

	public delegate void DialogueLifecycleEvent();
	public event DialogueLifecycleEvent onDialogueFinished;
	
	private DialogueSet[] dialogueSets;		// Dialogue data
	private bool proceed;								
	private bool willAcceptScreenPress;
	private Coroutine displayDialogueRoutine;

	void OnDisable() {
		for (int i = 0; i < namePanels.Length; i ++) {
			speakerImages[i].gameObject.SetActive(false);
			namePanels[i].alpha = 0;
		}
	}

	void OnEnable() {
		for (int i = 0; i < namePanels.Length; i ++) {
			speakerImages[i].gameObject.SetActive(true);
		}
	}

	public void Init(params DialogueSet[] dialogueSets)
	{
		willAcceptScreenPress = false;
		this.dialogueSets = dialogueSets;
		gameObject.SetActive(true);
		displayDialogueRoutine = StartCoroutine(DisplayDialogues());
	}

	private IEnumerator DisplayDialogues()
	{
		dialogueText.text = "";
		dialoguePlaying = true;

		// Initialize view
		speakerImages[dialogueSets[0].position].sprite = dialogueSets[0].character.GetExpression(dialogueSets[0].dialogues[0].expression);

		yield return new WaitForSeconds(0.5f);      // Wait for UI to animate in

		int i = 0;
		while (i < dialogueSets.Length)
		{
			// Initialize UI elements
			DialogueSet dialogueSet = dialogueSets[i];
			if (dialogueSet.firstAppearance) {
				speakerImages[dialogueSet.position].GetComponent<Animator>().SetFloat("Direction", 1);
				speakerImages[dialogueSet.position].GetComponent<Animator>().Play(SLIDE_IN_STATE[dialogueSet.position], -1, 0);
			}
			speakerImages[dialogueSet.position].color = Color.white;			
			namePanels[dialogueSet.position].alpha = 1f;
			nameTexts[dialogueSet.position].text = dialogueSet.character.characterName;
			nameTexts[dialogueSet.position].color = dialogueSet.character.nameColor;
			dialogueText.textBox.color = dialogueSet.character.textColor;
			dialogueText.SetScrollAudio(dialogueSet.character.voice);
			// Set non-speaker graphics to be darker
			for (int k = 0; k < nameTexts.Length; k ++) {
				if (k != dialogueSet.position) {
					// Only do this if this speaker has been introduced already
					if (namePanels[k].alpha > 0) {
						namePanels[k].alpha = 0.5f;
						speakerImages[k].color = Color.gray;
					}
				}
			}

			// Display dialogues
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
				while (!proceed)							// Continue on screen press
					yield return null;
				proceed = false;
				j++;
			}
			if (dialogueSet.exitAtEnd) {
				speakerImages[dialogueSet.position].GetComponent<Animator>().SetFloat("Direction", -2);
				speakerImages[dialogueSet.position].GetComponent<Animator>().Play(SLIDE_IN_STATE[dialogueSet.position], -1, 1);
				namePanels[dialogueSet.position].alpha = 0;
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
		if (d.expression != "") {
			speakerImages[dSet.position].sprite = dSet.character.GetExpression(d.expression);
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

	public void SkipDialogue() {
		StopCoroutine(displayDialogueRoutine);
		gameObject.SetActive(false);
		dialoguePlaying = false;
		if (onDialogueFinished != null)
			onDialogueFinished();
	}
}
