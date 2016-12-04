using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyWaveText : MonoBehaviour {

	public Text text;
	public Color defaultColor;

	public AudioClip victorySound;
	public AudioClip warningSound;

	private bool canDisplayNextMessage = true;

	public ParticleSystem waveCompleteParticles;

	public void DisplayWaveNumberAfterDelay (int waveNumber)
	{
		text.color = new Color (0, 0, 0, 0);
		StartCoroutine (DisplayWaveNum (waveNumber));
	}

	public void DisplayWaveComplete()
	{
		waveCompleteParticles.gameObject.SetActive (true);
		SoundManager.instance.PlayUISound (victorySound);
		StartCoroutine (FadeAway (Color.yellow, "Wave Complete"));
	}

	public void DisplayBossIncoming()
	{
		text.text = "Warning: Boss Incoming";
		StartCoroutine (Flash (3, Color.red));
	}

	private IEnumerator DisplayWaveNum(int waveNumber)
	{
		yield return new WaitForSeconds (1.0f);
		StartCoroutine (FadeAway (Color.white, "Wave " + waveNumber));
	}

	private IEnumerator FadeAway(Color color, string message)
	{
		yield return new WaitUntil (CanDisplayNextMessage);

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
		Debug.Log ("Stopped");
		text.color = finalColor;
		waveCompleteParticles.gameObject.SetActive (false);
	}

	private IEnumerator Flash(int numTimes, Color color)
	{
		//Debug.Log ("Started Boss");
		yield return new WaitUntil (CanDisplayNextMessage);
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
	}

	private bool CanDisplayNextMessage()
	{
		//Debug.Log (canDisplayNextMessage);
//		if (canDisplayNextMessage)
//			Debug.Log ("Fuck yeah");
		return canDisplayNextMessage;
	}
}
