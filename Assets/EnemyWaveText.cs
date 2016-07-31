using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyWaveText : MonoBehaviour {

	public Text text;

	public void Init (int waveNumber)
	{
		text.text = "Wave " + waveNumber;
		gameObject.SetActive (true);
		StartCoroutine (FadeAway ());
	}

	private IEnumerator FadeAway()
	{
		Color initialColor = new Color (1, 1, 1, 1);
		Color finalColor = new Color (1, 1, 1, 0);
		float t = 0;
		text.color = initialColor;
		while (text.color.a >= 0.05f)
		{
			t += Time.deltaTime;
			text.color = Color.Lerp (initialColor, finalColor, t);
			yield return null;
		}
		gameObject.SetActive (false);
	}
}
