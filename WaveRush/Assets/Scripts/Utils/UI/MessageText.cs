using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessageText : MonoBehaviour
{
	public bool displaying { get; private set; }
	private Text text;

	public delegate void FlashedMessage();
	public FlashedMessage OnFlashMessage;

	// Use this for initialization
	void Awake ()
	{
		text = GetComponent<Text> ();
	}

	public void SetColor(Color color)
	{
		text.color = color;
	}

	public void Display(string message, int numTimes, float persistTime, float fadeOutTime)
	{
		StopAllCoroutines ();
		StartCoroutine (FlashMessage (message, numTimes, persistTime, fadeOutTime));
	}

	private IEnumerator FlashMessage(string message, int numTimes, float persistTime, float fadeOutTime)
	{
		displaying = true;
		text.StopAllCoroutines ();	// stop any previous crossFadeAlpha in progress
		text.text = message;
		while (numTimes > 0)
		{
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

