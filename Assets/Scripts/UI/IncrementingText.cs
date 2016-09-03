using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IncrementingText : MonoBehaviour {

	public Text text;
	public int numberToReport;
	public AudioClip blipSound;

	void Awake()
	{
		text.text = "0";
	}

	public void ReportScore(int number)
	{
		numberToReport = number;
		StartCoroutine (DisplayNumber ());
	}

	private IEnumerator DisplayNumber()
	{
		int incrementer = 0;
		yield return new WaitForSeconds (1.0f);
		while (incrementer < numberToReport)
		{
			incrementer++;
			text.text = incrementer.ToString ();
			SoundManager.instance.PlayUISound (blipSound);
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
