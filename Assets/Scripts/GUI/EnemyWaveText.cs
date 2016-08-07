using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyWaveText : MonoBehaviour {

	public Text text;
	public Color defaultColor;
	public AudioClip warningSound;

	void Start()
	{
		defaultColor = text.color;
	}

	public void DisplayWaveNumber (int waveNumber)
	{
		text.text = "Wave " + waveNumber;
		gameObject.SetActive (true);
		StartCoroutine (FadeAway (defaultColor));
	}

	public void DisplayBossIncoming()
	{
		text.text = "Warning: Boss Incoming";
		gameObject.SetActive (true);
		StartCoroutine (Flash (3, Color.red));
	}

	private IEnumerator FadeAway(Color color)
	{
		Color initialColor = color;
		Color finalColor = new Color (color.r, color.b, color.g, 0);
		float t = 0;
		text.color = initialColor;
		yield return new WaitForSeconds (1.0f);
		while (text.color.a >= 0.05f)
		{
			t += Time.deltaTime;
			text.color = Color.Lerp (initialColor, finalColor, t);
			yield return null;
		}
		gameObject.SetActive (false);
	}

	private IEnumerator Flash(int numTimes, Color color)
	{
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
		gameObject.SetActive (false);
	}
}
