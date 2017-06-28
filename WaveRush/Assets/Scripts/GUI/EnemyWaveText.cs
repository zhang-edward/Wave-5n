using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyWaveText : MonoBehaviour {

	public MessageText messageText;
	public Color waveTextColor;
	public Color waveCompleteTextColor;
	public Color bossIncomingTextColor;

	public AudioClip victorySound;
	public AudioClip warningSound;

	public Coroutine waveRoutine;

	//private bool canDisplayNextMessage = true;

	public ParticleSystem waveCompleteParticles;

	public void DisplayWaveNumberAfterDelay (int waveNumber, float delay)
	{
		StartCoroutine (DisplayMessageDelayed ("Wave " + waveNumber, waveTextColor, 1f));
	}

	public void DisplayWaveComplete()
	{
		if (waveRoutine != null)
			StopCoroutine(waveRoutine);
		waveRoutine = StartCoroutine (DisplayMessageInterrupt("Wave Complete", 
			waveCompleteTextColor,
			WaveCompleteEffect));
	}

	private void WaveCompleteEffect()
	{
		waveCompleteParticles.gameObject.SetActive (true);
		waveCompleteParticles.Play ();
		SoundManager.instance.PlayUISound (victorySound);
	}

	public void DisplayBossIncoming()
	{
		StartCoroutine (DisplayMessage("Warning: Boss Incoming", 
			bossIncomingTextColor, 
			BossIncomingEffect, 
			numTimes: 3, 
			persistTime: 0.5f, 
			fadeOutTime: 0.5f));
	}

	private void BossIncomingEffect()
	{
		SoundManager.instance.PlayUISound (warningSound);
	}

	private IEnumerator DisplayMessageDelayed(string message, Color color, float delay, 
		MessageText.FlashedMessage callback = null, int numTimes = 1, float persistTime = 2f, float fadeOutTime = 0.2f)
	{
		while (messageText.displaying)
			yield return null;
		messageText.displaying = true;
		yield return new WaitForSecondsRealtime (delay);

		messageText.SetColor (color);
		messageText.Display (message, numTimes, persistTime, fadeOutTime);
	}

	private IEnumerator DisplayMessage(string message, Color color,
		MessageText.FlashedMessage callback = null, int numTimes = 1, float persistTime = 2f, float fadeOutTime = 0.2f)
	{
		while (messageText.displaying)
			yield return null;

		messageText.OnFlashMessage = callback;
		messageText.SetColor (color);
		messageText.Display (message, numTimes, persistTime, fadeOutTime);
	}

	private IEnumerator DisplayMessageInterrupt(string message, Color color,
		MessageText.FlashedMessage callback = null, int numTimes = 1, float persistTime = 2f, float fadeOutTime = 0.2f)
	{
		messageText.OnFlashMessage = callback;
		messageText.SetColor (color);
		messageText.Display (message, numTimes, persistTime, fadeOutTime);
		yield return null;
	}

	/*private IEnumerator FlashMessage(string message, int numTimes = 1, float fadeOutTime = 1f)
	{
		text.text = message;
		// wait for the current message (if any) to be completed
		while (!canDisplayNextMessage)
		{
			Debug.Log ("Blocked by current message!");
			yield return null;
		}	
		// we are currently displaying a message
		canDisplayNextMessage = false;
		while (numTimes > 0)
		{
			yield return new WaitForSecondsRealtime (1f);
			text.CrossFadeAlpha (0, fadeOutTime, true);	// fade out

			yield return new WaitForSecondsRealtime (fadeOutTime);
			numTimes--;
		}
		canDisplayNextMessage = true;
	}

	private IEnumerator DisplayWaveNum(int waveNumber, float delay)
	{
		yield return new WaitForSeconds (delay);
		text.color = defaultColor;
		StartCoroutine (FlashMessage ("Wave " + waveNumber));
	}

	private IEnumerator FadeAway(Color color, string message)
	{
		//yield return new WaitUntil (CanDisplayNextMessage);

		text.text = message;
		canDisplayNextMessage = false;
		Color initialColor = color;
		Color finalColor = new Color (color.r, color.b, color.g, 0);
		float t = 0;
		text.color = initialColor;
		yield return new WaitForSeconds (0.5f);
		while (text.color.a >= 0.05f)
		{
			t += Time.deltaTime;
			text.color = Color.Lerp (initialColor, finalColor, t);
			yield return null;
		}
		canDisplayNextMessage = true;
		//Debug.Log ("Stopped");
		text.color = finalColor;
		waveCompleteParticles.gameObject.SetActive (false);
	}

	private IEnumerator Flash(int numTimes, Color color)
	{
		//Debug.Log ("Started Boss");
		//yield return new WaitUntil (CanDisplayNextMessage);
		//Debug.Log ("Continuing Boss");

		canDisplayNextMessage = false;
		Color initialColor = color;
		Color finalColor = new Color (color.r, color.b, color.g, 0);
		float t = 0;
		int timesFlashed = 0;
		text.color = initialColor;
		while (timesFlashed < numTimes)
		{
			SoundManager.instance.PlayImportantSound (warningSound);
			while (text.color.a >= 0.05f)
			{
				t += Time.deltaTime;
				text.color = Color.Lerp (initialColor, finalColor, t);
				yield return null;
			}
			timesFlashed++;
			t = 0;
			text.color = initialColor;
		}
		canDisplayNextMessage = true;

		text.color = finalColor;
	}*/
}
