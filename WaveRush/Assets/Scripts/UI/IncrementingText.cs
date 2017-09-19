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

	public bool doneUpdating { get; private set; }

	private int numberToReport;
	private float pitch;

	void Awake()
	{
		text = GetComponent<TMP_Text>();
		//audioSrc = GetComponent<AudioSource> ();
		text.text = initialValue.ToString();
		doneUpdating = true;
	}

	public void DisplayNumber(int number)
	{
		numberToReport = number;
		StartCoroutine (DisplayNumber ());
	}

	private IEnumerator DisplayNumber()
	{
		doneUpdating = false;
		//Debug.Log ("moneyEarned: " + text.text);
		int incrementer = int.Parse(text.text.ToString());
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
			if (audioSrc != null)
				PlayAudio(incrementer);
			text.text = incrementer.ToString ();

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
			text.text = numberToReport.ToString ();
		}
	}
}
