using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class IncrementingText : MonoBehaviour {

	public Text text;
	public int numberToReport;
	public AudioClip blipSound;
	public AudioSource audioSrc;

	private float pitch;

	void Awake()
	{
		audioSrc = GetComponent<AudioSource> ();
		//text.text = "0";
	}

	public void ReportScore(int number)
	{
		numberToReport = number;
		StartCoroutine (DisplayNumber ());
	}

	private IEnumerator DisplayNumber()
	{
		//Debug.Log ("moneyEarned: " + text.text);
		int incrementer = int.Parse(text.text.ToString());
		yield return new WaitForSeconds (1.0f);
		while (incrementer != numberToReport)
		{
			if (Mathf.Abs(numberToReport - incrementer) > 10)
			{
				incrementer = (int)Mathf.Lerp (incrementer, numberToReport, 0.5f);			
			}
			else
			{
				incrementer += ((int)Mathf.Sign (numberToReport - incrementer));
			}
			
			text.text = incrementer.ToString ();
			pitch = ((float)incrementer / numberToReport) + 1;
			audioSrc.pitch = pitch;
			audioSrc.clip = blipSound;
			audioSrc.Play ();
			yield return new WaitForSeconds (0.03f);
		}
		yield return null;
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			StopAllCoroutines ();
			text.text = numberToReport.ToString ();
		}
	}
}
