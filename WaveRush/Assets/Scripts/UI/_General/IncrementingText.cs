using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class IncrementingText : MonoBehaviour {

	public TMP_Text text { get; private set; }
	public bool alterPitch = true;
	public AudioClip blipSound;
	public AudioSource audioSrc;
	public int initialValue;
	public string prefix;
	public string postfix;

	public bool doneUpdating { get; private set; }

	private int numberToReport;
	private float pitch;

	void Awake()
	{
		text = GetComponent<TMP_Text>();
		//audioSrc = GetComponent<AudioSource> ();
		text.text = prefix + initialValue.ToString() + postfix;
		doneUpdating = true;
	}

	void OnEnable()
	{
		if (audioSrc != null)
			SoundManager.instance.RegisterSfxSrc(audioSrc);
	}

	void OnDisable()
	{
		if (audioSrc != null)
			SoundManager.instance.UnregisterSfxSrc(audioSrc);
	}

	public void DisplayNumber(int number, string prefix = "", string postfix = "") {
		if (prefix != "")
			this.prefix = prefix;
		if (postfix != "")
			this.postfix = postfix;

		numberToReport = number;
		StartCoroutine (DisplayNumber ());
	}

	private IEnumerator DisplayNumber()
	{
		doneUpdating = false;
		//Debug.Log ("moneyEarned: " + text.text);
		int incrementer = int.Parse(text.text.ToString());
		int audioCounter = 0;
		while (incrementer != numberToReport)
		{
			if (Mathf.Abs(numberToReport - incrementer) > 50)
			{
				incrementer = (int)Mathf.Lerp (incrementer, numberToReport, 0.05f);			
			}
			else
			{
				incrementer += ((int)Mathf.Sign (numberToReport - incrementer));
			}
			audioCounter++;
			if (audioSrc != null && audioCounter % 3 == 0)
				PlayAudio(incrementer);
			text.text = prefix + incrementer.ToString () + postfix;

			yield return new WaitForSeconds (0.03f);
		}
		doneUpdating = true;
		yield return null;
	}

	private void PlayAudio(int incrementer)
	{
		if (alterPitch)
		{
			pitch = ((float)incrementer / numberToReport) + 0.5f;
			audioSrc.pitch = pitch;
		}
		audioSrc.clip = blipSound;
		audioSrc.Play();
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0) && doneUpdating == false)
		{
			StopAllCoroutines ();
			doneUpdating = true;
			text.text = prefix + numberToReport.ToString () + postfix; 
		}
	}
}
