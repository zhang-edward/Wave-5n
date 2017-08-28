﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MessageText : MonoBehaviour
{
	[System.Serializable]
	public class Message
	{
		public string message;
		public int numTimes = 1;
		public float fadeInTime = 0f;
		public float persistTime = 2.0f;
		public float fadeOutTime = 0.2f;
		public Color color = Color.white;

		public float totalMessageTime {
			get{
				return persistTime + fadeOutTime;
			}
		}

		public Message(string message, int numTimes, float fadeInTime, float persistTime, float fadeOutTime, Color color)
		{
			this.message = message;
			this.numTimes = numTimes;
			this.fadeInTime = fadeInTime;
			this.persistTime = persistTime;
			this.fadeOutTime = fadeOutTime;
		}

		public Message(string message)
		{
			this.message = message;
		}
	}
		

	public bool displaying;
	private TMP_Text text;

	public delegate void FlashedMessage();
	public FlashedMessage OnFlashMessage;

	// Use this for initialization
	void Awake ()
	{
		text = GetComponent<TMP_Text> ();
	}

	public void SetColor(Color color)
	{
		text.color = color;
	}

	public void Display(Message msg)
	{
		StopAllCoroutines ();
		StartCoroutine (FlashMessage (msg.message, msg.numTimes, msg.fadeInTime, msg.persistTime, msg.fadeOutTime));
	}

	private IEnumerator FlashMessage(string message, int numTimes, float fadeIntime, float persistTime, float fadeOutTime)
	{
		displaying = true;
		text.StopAllCoroutines ();	// stop any previous crossFadeAlpha in progress
		text.text = message;
		while (numTimes > 0)
		{
			text.canvasRenderer.SetAlpha(0);
			text.CrossFadeAlpha(1, fadeIntime, true);
			yield return new WaitForSecondsRealtime(fadeIntime);
			text.canvasRenderer.SetAlpha (1);
			if (OnFlashMessage != null)
				OnFlashMessage ();
			yield return new WaitForSecondsRealtime (persistTime);
			text.CrossFadeAlpha (0, fadeOutTime, true);	// fade out

			yield return new WaitForSecondsRealtime (fadeOutTime + 0.1f);
			numTimes--;
		}
		displaying = false;
		OnFlashMessage = null;
	}
}

